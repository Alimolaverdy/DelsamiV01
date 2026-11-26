using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Models;
using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Web.Areas.Admin.Factories;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Services.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Misc.Core.Services.CityService;


namespace Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Factories
{
    public class DelsamiShippingModelFactory : IDelsamiShippingModelFactory
    {
        #region Fields
        private readonly IDelsamiStateProvinceService _DelsamiStateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IDelsamiShippingService _DelsamiShippingService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ISettingService _settingService;
        private readonly ICityService _cityService;
        #endregion

        #region Ctor
        public DelsamiShippingModelFactory(IDelsamiStateProvinceService DelsamiStateProvinceService,
                                            IShippingService shippingService,
                                            IDelsamiShippingService DelsamiShippingService,
                                            IBaseAdminModelFactory baseAdminModelFactory,
                                            ICityService cityService,
                                            ISettingService settingService
)
        {
            _DelsamiStateProvinceService = DelsamiStateProvinceService;
            _shippingService = shippingService;
            _DelsamiShippingService = DelsamiShippingService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _settingService = settingService;
            _cityService = cityService;
        }


        #endregion

        #region Methods

        public async Task<ConfigurationModel> PrepareConfigureModelAsync(ConfigurationModel configurationModel)
        {
            await _baseAdminModelFactory.PrepareCountriesAsync(configurationModel.AvailableCountries);
            return configurationModel;

        }
        public async Task<DelsamiShippingMethodStateProvinceRestrictionModel> PrepareDelsamiShippingMethodRestrictionModelAsync(DelsamiShippingMethodStateProvinceRestrictionModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var setting = await _settingService.LoadSettingAsync<ManageShippingMethodSetting>();
            var selectedCountryId = setting.CountryId;

            var stateProvinces = await _DelsamiStateProvinceService.GetAllStateProvincesByCountryIdAsync(selectedCountryId);

            model.SelectedStateProviceId= stateProvinces.FirstOrDefault(a=>a.Name=="تهران").Id;
            model.AvailableStateProvinces = stateProvinces.Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();

            return model;
        }

        public async Task<DelsamiShippingMethodStateProvinceRestrictionModel> PrepareDelsamiShippingMethodRestrictionModelAsync(int stateProvinceId)
        {
            var cities = await _cityService.GetCitiesByStateProvinceIdAsync(stateProvinceId);
            var model = new DelsamiShippingMethodStateProvinceRestrictionModel();

            model.AvailableCities = cities;


            foreach (var shippingMethod in await _shippingService.GetAllShippingMethodsAsync())
            {
                model.AvailableShippingMethods.Add(shippingMethod.ToModel<ShippingMethodModel>());
                foreach (var city in cities)
                {
                    if (!model.Restricted.ContainsKey(city.Id))
                        model.Restricted[city.Id] = new Dictionary<int, bool>();

                    model.Restricted[city.Id][shippingMethod.Id] = await _DelsamiShippingService.CityRestrictionExistsAsync(shippingMethod, city.Id);
                }
            }

            return model;
        }
        #endregion
    }
}
