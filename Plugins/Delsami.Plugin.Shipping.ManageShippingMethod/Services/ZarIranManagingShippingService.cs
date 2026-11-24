using ZarIran.Plugin.Shipping.Core.Services;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Factories;
using ZarIran.Plugin.Widgets.AddressLocation.Services;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Services
{
    public class ZarIranManagingShippingService : ShippingService
    {

        #region Fields

        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly ILogger _logger;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShippingSettings _shippingSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IZarIranShippingService _ZarIranShippingService;
        private readonly ManageShippingMethodSetting _manageShippingMethodSetting;
        private readonly IAddressCityService _addressCityService;
        #endregion

        #region Ctor
        public ZarIranManagingShippingService(IAddressService addressService,
                                       ICheckoutAttributeParser checkoutAttributeParser,
                                       ICountryService countryService,
                                       IAddressCityService addressCityService,
                                       ICustomerService customerService,
                                       IGenericAttributeService genericAttributeService,
                                       ILocalizationService localizationService,
                                       ILogger logger,
                                       IPickupPluginManager pickupPluginManager,
                                       IPriceCalculationService priceCalculationService,
                                       IProductAttributeParser productAttributeParser,
                                       IProductService productService,
                                       IRepository<ShippingMethod> shippingMethodRepository,
                                       IRepository<ShippingMethodCountryMapping> shippingMethodCountryMappingRepository,
                                       IRepository<Warehouse> warehouseRepository,
                                       IShippingPluginManager shippingPluginManager,
                                       IStateProvinceService stateProvinceService,
                                       IStoreContext storeContext,
                                       ShippingSettings shippingSettings,
                                       IZarIranShippingService ZarIranShippingService,
                                       ShoppingCartSettings shoppingCartSettings,
                                       ManageShippingMethodSetting manageShippingMethodSetting) : base(addressService,
                                                                                         checkoutAttributeParser,
                                                                                         countryService,
                                                                                         customerService,
                                                                                         genericAttributeService,
                                                                                         localizationService,
                                                                                         logger,
                                                                                         pickupPluginManager,
                                                                                         priceCalculationService,
                                                                                         productAttributeParser,
                                                                                         productService,
                                                                                         shippingMethodRepository,
                                                                                         shippingMethodCountryMappingRepository,
                                                                                         warehouseRepository,
                                                                                         shippingPluginManager,
                                                                                         stateProvinceService,
                                                                                         storeContext,
                                                                                         shippingSettings,
                                                                                         shoppingCartSettings)
        {
            _shippingPluginManager = shippingPluginManager;
            _addressCityService = addressCityService;
            _logger = logger;
            _shoppingCartSettings = shoppingCartSettings;
            _priceCalculationService = priceCalculationService;
            _shippingSettings = shippingSettings;
            _localizationService = localizationService;
            _ZarIranShippingService = ZarIranShippingService;
            _manageShippingMethodSetting = manageShippingMethodSetting;
        }
        #endregion

        #region Methods

        public override async Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart,
                                                                                      Address shippingAddress,
                                                                                      Customer customer = null,
                                                                                      string allowedShippingRateComputationMethodSystemName = "",
                                                                                      int storeId = 0)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var result = new GetShippingOptionResponse();

            var customerCityId = (await _addressCityService.GetAddressCityByAddressIdAsync(shippingAddress.Id))?.CityId ?? 0;

            //create a package
            var (shippingOptionRequests, shippingFromMultipleLocations) = await CreateShippingOptionRequestsAsync(cart, shippingAddress, storeId);
            result.ShippingFromMultipleLocations = shippingFromMultipleLocations;

            var shippingRateComputationMethods = await _shippingPluginManager
                .LoadActivePluginsAsync(customer, storeId, allowedShippingRateComputationMethodSystemName);
            if (!shippingRateComputationMethods.Any())
                return result;

            //request shipping options from each shipping rate computation methods
            foreach (var srcm in shippingRateComputationMethods)
            {
                //request shipping options (separately for each package-request)
                IList<ShippingOption> srcmShippingOptions = null;
                foreach (var shippingOptionRequest in shippingOptionRequests)
                {

                    var getShippingOptionResponse = await srcm.GetShippingOptionsAsync(shippingOptionRequest);

                    if (getShippingOptionResponse.Success)
                    {
                        //success
                        if (srcmShippingOptions == null)
                        {
                            //first shipping option request
                            srcmShippingOptions = getShippingOptionResponse.ShippingOptions;
                        }
                        else
                        {
                            //get shipping options which already exist for prior requested packages for this scrm (i.e. common options)
                            srcmShippingOptions = srcmShippingOptions
                                .Where(existingso => getShippingOptionResponse.ShippingOptions.Any(newso => newso.Name == existingso.Name))
                                .ToList();

                            //and sum the rates
                            foreach (var existingso in srcmShippingOptions)
                            {
                                existingso.Rate += getShippingOptionResponse
                                    .ShippingOptions
                                    .First(newso => newso.Name == existingso.Name)
                                    .Rate;
                            }
                        }
                    }
                    else
                    {
                        //errors
                        foreach (var error in getShippingOptionResponse.Errors)
                        {
                            result.AddError(error);
                            await _logger.WarningAsync($"Shipping ({srcm.PluginDescriptor.FriendlyName}). {error}");
                        }
                        //clear the shipping options in this case
                        srcmShippingOptions = new List<ShippingOption>();
                        break;
                    }
                }

                //add this scrm's options to the result
                if (srcmShippingOptions == null)
                    continue;

                foreach (var so in srcmShippingOptions)
                {

                    //set system name if not set yet
                    if (string.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName))
                        so.ShippingRateComputationMethodSystemName = srcm.PluginDescriptor.SystemName;
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        so.Rate = await _priceCalculationService.RoundPriceAsync(so.Rate);
                    result.ShippingOptions.Add(so);
                }
            }

            if (_shippingSettings.ReturnValidOptionsIfThereAreAny)
            {
                //return valid options if there are any (no matter of the errors returned by other shipping rate computation methods).
                if (result.ShippingOptions.Any() && result.Errors.Any())
                    result.Errors.Clear();
            }

            //no shipping options loaded
            if (!result.ShippingOptions.Any() && !result.Errors.Any())
                result.Errors.Add(await _localizationService.GetResourceAsync("Checkout.ShippingOptionCouldNotBeLoaded"));
            var disabledShippingMethods = !string.IsNullOrWhiteSpace(_manageShippingMethodSetting.DisabledShippingMethods) ? _manageShippingMethodSetting.DisabledShippingMethods.Split(",")?.Select(int.Parse).ToList() : new List<int>();
            foreach (var shippingOption in result.ShippingOptions.ToList())
            {

                var shippingMethod = await _ZarIranShippingService.GetShippingMethodByNameAsync(shippingOption.Name);
                bool isRestricted = await _ZarIranShippingService.CityRestrictionExistsAsync(shippingMethod, customerCityId);

                if (!isRestricted)
                    isRestricted = disabledShippingMethods.Any(a => a == shippingMethod.Id);

                if (isRestricted)
                    result.ShippingOptions.Remove(shippingOption);
            }

            return result;
        }

        #endregion

    }
}