using ZarIran.Plugin.Shipping.Core.Helpers;
using ZarIran.Plugin.Shipping.Core.Services;
using ZarIran.Plugin.Shipping.Tipax.Factories;
using ZarIran.Plugin.Widgets.AddressLocation;
using ZarIran.Plugin.Widgets.AddressLocation.Services;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Tipax.Services
{
    public class TipaxService : ITipaxService
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ITipaxModelsFactory _tipaxModelsFactory;
        private readonly ITipaxApiService _tipaxApiService;
        private readonly IVendorService _vendorService;
        private readonly IAddressService _addressService;
        private readonly AddressLocationSettings _addressLocationSettings;
        private readonly IAddressCityService _addressCityService;
        private readonly ICartonService _cartonService;
        private readonly ILogger _logger;
        private readonly IShippingService _shippingService;
        private readonly TipaxSettings _tipaxSettings;

        #endregion

        #region Ctor

        public TipaxService(AddressSettings addressSettings,
                            IAddressModelFactory addressModelFactory,
                            ITipaxModelsFactory tipaxModelsFactory,
                            ITipaxApiService tipaxApiService,
                            IVendorService vendorService,
                            IAddressService addressService,
                            AddressLocationSettings addressLocationSettings,
                            IAddressCityService addressCityService,
                            ICartonService cartonService,
                            ILogger logger,
                            IShippingService shippingService,
                            TipaxSettings tipaxSettings)
        {
            _addressSettings = addressSettings;
            _addressModelFactory = addressModelFactory;
            _tipaxModelsFactory = tipaxModelsFactory;
            _tipaxApiService = tipaxApiService;
            _vendorService = vendorService;
            _addressService = addressService;
            _addressLocationSettings = addressLocationSettings;
            _addressCityService = addressCityService;
            _cartonService = cartonService;
            _logger = logger;
            _shippingService = shippingService;
            _tipaxSettings = tipaxSettings;
        }

        #endregion

        #region Methods

        public async Task<List<ShippingOption>> PrepareShippingOptionsAsync(Customer customer, Address address, IList<Product> products)
        {
            List<ShippingOption> shippingOptions = new();

            try
            {

                var shippingAddressModel = new AddressModel();

                await _addressModelFactory.PrepareAddressModelAsync(shippingAddressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    prePopulateWithCustomerFields: true,
                    customer: customer);

                var shippingAddressLatitude = shippingAddressModel.CustomAddressAttributes.FirstOrDefault(x => x.Id == _addressLocationSettings.LatitudeAddressAttributeId)?.DefaultValue;
                var shippingAddressLongitude = shippingAddressModel.CustomAddressAttributes.FirstOrDefault(x => x.Id == _addressLocationSettings.LongitudeAddressAttributeId)?.DefaultValue;

                if (shippingAddressLatitude == null)
                    return shippingOptions;
                //throw new Exception("shippingAddressLatitude not found");

                if (shippingAddressLongitude == null)
                    return shippingOptions;
                //throw new Exception("shippingAddressLongitude not found");

                var destinationAddressCity = await _addressCityService.GetAddressCityByAddressIdAsync(shippingAddressModel.Id);

                if (destinationAddressCity is not null)
                {
                    var weight = products.Sum(s => s.Weight);
                    var price = products.Sum(s => s.Price);

                    var cartons = await _cartonService.GetCartonsAsync();

                    if (cartons is not null && cartons.Any())
                    {
						var packingResult = PackagingHelper.PackProducts(cartons, products);

                        var length = packingResult.Select(s => s.Carton).Sum(s => s.Length);
                        var width = packingResult.Select(s => s.Carton).Sum(s => s.Width);
                        var height = packingResult.Select(s => s.Carton).Sum(s => s.Height);

                        var pricingRequestModel = await _tipaxModelsFactory.PreparePricingRequestModelAsync(shippingAddressModel.Id, weight, price, length, width, height);

                        if (pricingRequestModel is not null)
                        {
                            var tokenRequest = _tipaxModelsFactory.PrepareTokenRequestModelAsync();

                            var tokenResult = await _tipaxApiService.GetTokenAsync(tokenRequest);

                            var tipaxPricings = await _tipaxApiService.GetPricingAsync(pricingRequestModel, tokenResult.accessToken);

                            var finalPrice = tipaxPricings.FirstOrDefault().regularRate.finalPrice;
                            var shippingMethodId = _tipaxSettings.ShippingMethodId;

                            if (_tipaxSettings.Multiplier != 0)
                                finalPrice *= _tipaxSettings.Multiplier;

                            var rounded = Math.Ceiling(finalPrice / 10) * 10;

                            var shippingMethod = await _shippingService.GetShippingMethodByIdAsync(shippingMethodId);


                            shippingOptions.Add(new ShippingOption
                            {
                                Name = shippingMethod.Name,
                                Rate = rounded,
								DisplayOrder = shippingMethod.DisplayOrder,

							});
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }
            return shippingOptions;
        }

        #endregion
    }
}
