using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Shipping.Tipax.Models.TipaxApi;
using ZarIran.Plugin.Shipping.Tipax.Services;
using ZarIran.Plugin.Widgets.AddressLocation.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Services.CityService;

namespace ZarIran.Plugin.Shipping.Tipax.Factories
{
    public class TipaxModelsFactory : ITipaxModelsFactory
    {
        #region Fields

        private readonly TipaxSettings _tipaxSettings;
        private readonly ICityService _cityService;
        private readonly IZarIranTipaxCityMappingService _ZarIranTiapxCityMappingService;
        private readonly IAddressCityService _addressCityService;

        #endregion

        #region Ctor

        public TipaxModelsFactory(TipaxSettings tipaxSettings,
                                  ICityService cityService,
                                  IZarIranTipaxCityMappingService ZarIranTiapxCityMappingService,
                                  IAddressCityService addressCityService)
        {
            _tipaxSettings = tipaxSettings;
            _cityService = cityService;
            _ZarIranTiapxCityMappingService = ZarIranTiapxCityMappingService;
            _addressCityService = addressCityService;
        }

        #endregion

        #region Methods

        public async Task<PricingRequestModel> PreparePricingRequestModelAsync(int shippingAddressId,
                                                                               decimal weight,
                                                                               decimal packageValue,
                                                                               decimal length,
                                                                               decimal width,
                                                                               decimal height)
        {
            var requestModel = new PricingItemRequestModel();
            var tehranCity = await _cityService.GetCityByNameAsync("تهران");

            if (tehranCity is null)
                throw new ArgumentNullException(nameof(tehranCity));

            var addressCity = await _addressCityService.GetAddressCityByAddressIdAsync(shippingAddressId);

            if (addressCity is null)
                throw new ArgumentNullException(nameof(addressCity));

            var originZarIranTipaxCityMapping = await _ZarIranTiapxCityMappingService.GetZarIranTiapxCityMappingByZarIranCityIdAsync(tehranCity.Id);

            if (originZarIranTipaxCityMapping is null)
                return null;

            var destinationZarIranTipaxCityMapping = await _ZarIranTiapxCityMappingService.GetZarIranTiapxCityMappingByZarIranCityIdAsync(addressCity.CityId);

            if (destinationZarIranTipaxCityMapping is null)
                return null;

            var origin = new PricingItemRequestModel.Route
            {
                cityId = originZarIranTipaxCityMapping.TipaxCityId
            };


            var destination = new PricingItemRequestModel.Route
            {
                cityId = destinationZarIranTipaxCityMapping.TipaxCityId
            };

            requestModel.origin = origin;
            requestModel.destination = destination;
            requestModel.weight = weight / 1000;
            requestModel.length = length;
            requestModel.width = width;
            requestModel.height = height;
            requestModel.distributionType = _tipaxSettings.DistributionType;
            requestModel.packageContentId = _tipaxSettings.PackageContent;
            requestModel.packingId = _tipaxSettings.Packing;
            requestModel.packType = _tipaxSettings.PackType;
            requestModel.paymentType = _tipaxSettings.PaymentType;
            requestModel.pickupType = _tipaxSettings.PickupType;
            requestModel.serviceId = _tipaxSettings.ServiceType;

            return new PricingRequestModel
            {
                packageInputs = new List<PricingItemRequestModel> { requestModel }
            };
        }
        public TokenRequestModel PrepareTokenRequestModelAsync()
        {
            return new TokenRequestModel()
            {
                apiKey = _tipaxSettings.ApiKey,
                username = _tipaxSettings.Username,
                password = _tipaxSettings.Password,
            };
        }

        #endregion
    }
}
