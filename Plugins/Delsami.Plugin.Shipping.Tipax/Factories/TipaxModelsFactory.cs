using Delsami.Plugin.Misc.Core.Services;
using Delsami.Plugin.Shipping.Tipax.Models.TipaxApi;
using Delsami.Plugin.Shipping.Tipax.Services;
using Delsami.Plugin.Widgets.AddressLocation.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Services.CityService;

namespace Delsami.Plugin.Shipping.Tipax.Factories
{
    public class TipaxModelsFactory : ITipaxModelsFactory
    {
        #region Fields

        private readonly TipaxSettings _tipaxSettings;
        private readonly ICityService _cityService;
        private readonly IDelsamiTipaxCityMappingService _DelsamiTiapxCityMappingService;
        private readonly IAddressCityService _addressCityService;

        #endregion

        #region Ctor

        public TipaxModelsFactory(TipaxSettings tipaxSettings,
                                  ICityService cityService,
                                  IDelsamiTipaxCityMappingService DelsamiTiapxCityMappingService,
                                  IAddressCityService addressCityService)
        {
            _tipaxSettings = tipaxSettings;
            _cityService = cityService;
            _DelsamiTiapxCityMappingService = DelsamiTiapxCityMappingService;
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

            var originDelsamiTipaxCityMapping = await _DelsamiTiapxCityMappingService.GetDelsamiTiapxCityMappingByDelsamiCityIdAsync(tehranCity.Id);

            if (originDelsamiTipaxCityMapping is null)
                return null;

            var destinationDelsamiTipaxCityMapping = await _DelsamiTiapxCityMappingService.GetDelsamiTiapxCityMappingByDelsamiCityIdAsync(addressCity.CityId);

            if (destinationDelsamiTipaxCityMapping is null)
                return null;

            var origin = new PricingItemRequestModel.Route
            {
                cityId = originDelsamiTipaxCityMapping.TipaxCityId
            };


            var destination = new PricingItemRequestModel.Route
            {
                cityId = destinationDelsamiTipaxCityMapping.TipaxCityId
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
