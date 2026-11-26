using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.ShippingMethodRestriction;
using Delsami.Plugin.Payments.ManagePaymentMethod.Services;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Areas.Admin.Models.Shipping;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Services.CityService;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories
{
    public class DelsamiPaymentMethodModelFactory : IDelsamiPaymentMethodModelFactory
    {
        #region Fields

        private readonly IDelsamiStateProvinceService _DelsamiStateProvinceService;
        private readonly IPaymentMethodRestrictionService _DelsamiPaymentMethodService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerRoleModelFactory _customerRoleModelFactory;
        private readonly IPaymentMethodCustomerRoleRestrictionService _roleRestrictionService;
        private readonly ISettingService _settingService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly ICityService _cityService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentMethodShippingRestrictionService _paymentMethodShippingRestrictionService;


        #endregion

        #region Ctor
        public DelsamiPaymentMethodModelFactory(IDelsamiStateProvinceService DelsamiStateProvinceService,
                                            IPaymentMethodRestrictionService DelsamiPaymentMethodService,
                                            IBaseAdminModelFactory baseAdminModelFactory,
                                            ICityService cityService,
                                            IPaymentMethodShippingRestrictionService paymentMethodShippingRestrictionService,
                                            IShippingService shippingService,
                                            ICustomerRoleModelFactory customerRoleModelFactory,
                                            ISettingService settingService,
                                            IPaymentMethodCustomerRoleRestrictionService roleRestrictionService,
                                            IPaymentPluginManager paymentPluginManager)
        {
            _DelsamiStateProvinceService = DelsamiStateProvinceService;
            _DelsamiPaymentMethodService = DelsamiPaymentMethodService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _settingService = settingService;
            _roleRestrictionService = roleRestrictionService;
            _customerRoleModelFactory = customerRoleModelFactory;
            _paymentPluginManager = paymentPluginManager;
            _cityService = cityService;
            _shippingService = shippingService;
            _paymentMethodShippingRestrictionService = paymentMethodShippingRestrictionService;
        }


        #endregion

        #region Methods

        public async Task<ConfigurationModel> PrepareConfigureModelAsync(ConfigurationModel configurationModel)
        {
            await _baseAdminModelFactory.PrepareCountriesAsync(configurationModel.AvailableCountries);
            return configurationModel;

        }
        public async Task<DelsamiPaymentMethodRestrictionModel> PrepareDelsamiPaymentMethodRestrictionModelAsync(DelsamiPaymentMethodRestrictionModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var setting = await _settingService.LoadSettingAsync<ManagePaymentMethodSetting>();
            var selectedCountryId = setting.CountryId;

            var stateProvinces = await _DelsamiStateProvinceService.GetAllStateProvincesByCountryIdAsync(selectedCountryId);

            model.SelectedStateProviceId = stateProvinces.FirstOrDefault(a => a.Name == "تهران").Id;
            model.AvailableStateProvinces = stateProvinces.Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();

            return model;

        }

        public async Task<PaymentMethodShippingRestrictionModel> PreparePaymentMethodShippingRestrictionModelAsync()
        {

            var model = new PaymentMethodShippingRestrictionModel();

            var paymentMethods = (await _paymentPluginManager.LoadActivePluginsAsync())
                                        .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                                        .ToList();

            var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();
            shippingMethods.ForEach(a => model.AvailableShippingMethods.Add(a.ToModel<ShippingMethodModel>()));

            foreach (var paymentMethod in paymentMethods)
            {
                var paymentMethodModel = paymentMethod.ToPluginModel<PaymentMethodModel>();
                model.AvailablePaymentMethods.Add(paymentMethodModel);
                foreach (var shippingMethod in shippingMethods)
                {
                    if (!model.Restricted.ContainsKey(shippingMethod.Id))
                        model.Restricted[shippingMethod.Id] = new Dictionary<string, bool>();

                    model.Restricted[shippingMethod.Id][paymentMethodModel.SystemName] = await _paymentMethodShippingRestrictionService.CityRestrictionExistsAsync(paymentMethodModel.SystemName, shippingMethod.Id);
                }
            }

            return model;

        }

        public async Task<DelsamiPaymentMethodRestrictionModel> PrepareDelsamiPaymentMethodRestrictionModelAsync(int stateProvinceId)
        {
            var cities = await _cityService.GetCitiesByStateProvinceIdAsync(stateProvinceId);
            var model = new DelsamiPaymentMethodRestrictionModel();

            model.AvailableCities = cities;

            var paymentMethods = (await _paymentPluginManager.LoadActivePluginsAsync())
                                        .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                                        .ToList();

            foreach (var paymentMethod in paymentMethods)
            {
                var paymentMethodModel = paymentMethod.ToPluginModel<PaymentMethodModel>();
                model.AvailablePaymentMethods.Add(paymentMethodModel);
                foreach (var city in cities)
                {
                    if (!model.Restricted.ContainsKey(city.Id))
                        model.Restricted[city.Id] = new Dictionary<string, bool>();

                    model.Restricted[city.Id][paymentMethodModel.SystemName] = await _DelsamiPaymentMethodService.CityRestrictionExistsAsync(paymentMethodModel.SystemName, city.Id);
                }
            }

            return model;
        }

        public async Task<DelsamiPaymentMethodCustomerRoleRestrictionModel> PrepareRoleRestrictionModelAsync()
        {
            var model = new DelsamiPaymentMethodCustomerRoleRestrictionModel();
            var allRoleRestrictions = (await _roleRestrictionService.GetAllCustomerRoleRestrictionsAsync())?.ToDictionary(a => (a.PaymentMethodSystemName, a.CustomerRoleId), b => b);
            model.AvailableRoles = (await _customerRoleModelFactory.PrepareCustomerRoleListModelAsync(new CustomerRoleSearchModel() { Length = 1000 })).Data.ToList();

            var paymentMethods = await (await _paymentPluginManager
            .LoadActivePluginsAsync())
            .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
            .ToListAsync();

            foreach (var paymentMethod in paymentMethods)
            {
                var paymentMethodModel = paymentMethod.ToPluginModel<PaymentMethodModel>();
                model.AvailablePaymentMethods.Add(paymentMethodModel);
                foreach (var role in model.AvailableRoles)
                {
                    if (!model.Restricted.ContainsKey(role.Id))
                        model.Restricted[role.Id] = new Dictionary<string, bool>();

                    model.Restricted[role.Id][paymentMethodModel.SystemName] = allRoleRestrictions.ContainsKey((paymentMethodModel.SystemName, role.Id));
                }
            }

            return model;
        }
        #endregion
    }
}
