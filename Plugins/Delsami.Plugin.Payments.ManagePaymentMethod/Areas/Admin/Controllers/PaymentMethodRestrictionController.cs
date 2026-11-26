using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories;
using Delsami.Plugin.Payments.ManagePaymentMethod.Infrastructure.Permissions;
using System.Linq;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Delsami.Plugin.Payments.ManagePaymentMethod.Services;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.AmountRestriction;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.ShippingMethodRestriction;
using Delsami.Plugin.Misc.Core.Services.CityService;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class PaymentMethodRestrictionController : BasePluginController
{
    #region Fields
    private readonly IPermissionService _permissionService;
    private readonly IDelsamiPaymentMethodModelFactory _DelsamiShippingModelFactory;
    private readonly IShippingService _shippingService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IDelsamiStateProvinceService _DelsamiStateProvinceService;
    private readonly IPaymentMethodRestrictionService _DelsamiPaymentMethodService;
    private readonly IPaymentMethodShippingRestrictionService  _paymentMethodShippingRestrictionService;
    private readonly ISettingService _settingService;
    private readonly ICityService _cityService;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IPaymentMethodAmountRestrictionService _paymentMethodAmountRestrictionService;

    #endregion

    #region Ctor

    public PaymentMethodRestrictionController(IPermissionService permissionService,
                                          IDelsamiPaymentMethodModelFactory DelsamiShippingModelFactory,
                                          IShippingService shippingService,
                                          ICityService cityService,
                                          INotificationService notificationService,
                                          IPaymentMethodShippingRestrictionService paymentMethodShippingRestrictionService,
                                          ILocalizationService localizationService,
                                          IDelsamiStateProvinceService DelsamiStateProvinceService,
                                          IPaymentMethodRestrictionService DelsamiPaymentMethodService,
                                          ISettingService settingService,
                                          IPaymentPluginManager paymentPluginManager,
                                          IPaymentMethodAmountRestrictionService paymentMethodAmountRestrictionService)
    {
        _permissionService = permissionService;
        _paymentMethodShippingRestrictionService = paymentMethodShippingRestrictionService;
        _cityService = cityService;
        _DelsamiShippingModelFactory = DelsamiShippingModelFactory;
        _shippingService = shippingService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _DelsamiStateProvinceService = DelsamiStateProvinceService;
        _DelsamiPaymentMethodService = DelsamiPaymentMethodService;
        _settingService = settingService;
        _paymentPluginManager = paymentPluginManager;
        _paymentMethodAmountRestrictionService = paymentMethodAmountRestrictionService;
    }
    #endregion

    #region Methods

    #region Config

    public async Task<ActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return Content("Access denied");

        var setting = await _settingService.LoadSettingAsync<ManagePaymentMethodSetting>();
        var selectedCountryId = setting.CountryId;

        var model = await _DelsamiShippingModelFactory.PrepareConfigureModelAsync(new ConfigurationModel
        {
            CountryId = selectedCountryId,
        });


        return View("~/Plugins/Delsami.Payments.ManagePaymentMethod/Areas/Admin/Views/ManagePaymentMethod/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return Content("Access denied");

        var settings = await _settingService.LoadSettingAsync<ManagePaymentMethodSetting>();
        settings.CountryId = model.CountryId;

        await _settingService.SaveSettingAsync(settings);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

        return RedirectToAction("Configure");
    }

    #endregion


    #region City Restrication



    public virtual async Task<IActionResult> Restrictions()
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return AccessDeniedView();

        //prepare model
        var model = await _DelsamiShippingModelFactory.PrepareDelsamiPaymentMethodRestrictionModelAsync(new DelsamiPaymentMethodRestrictionModel());

        return View("~/Plugins/Delsami.Payments.ManagePaymentMethod/Areas/Admin/Views/ManagePaymentMethod/Restrictions.cshtml", model);
    }


    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("Restrictions")]
    public virtual async Task<IActionResult> RestrictionSave(DelsamiPaymentMethodRestrictionModel model, IFormCollection form)
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return AccessDeniedView();

        var cities = await _cityService.GetAllByProvinceId(model.SelectedStateProviceId);


        var paymentMethods = (await _paymentPluginManager.LoadActivePluginsAsync())
                              .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                              .ToList();

        foreach (var paymentMethod in paymentMethods)
        {
            var paymentMethodModel = paymentMethod.ToPluginModel<PaymentMethodModel>();

            var formKey = "restrict_" + paymentMethodModel.SystemName;
            var CityIdsToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            foreach (var city in cities)
            {
                var restrict = CityIdsToRestrict.Contains(city.Id);
                var paymentMethodCityMappings = await _DelsamiPaymentMethodService.GetPaymentMethodCityMappingAsync(paymentMethodModel.SystemName, city.Id);

                if (restrict)
                {
                    if (paymentMethodCityMappings.Any())
                        continue;

                    await _DelsamiPaymentMethodService.InsertPaymentMethodRestrictionAsync(new PaymentMethodCityRestriction { CityId = city.Id, PaymentMethodSystemName = paymentMethodModel.SystemName });
                }
                else
                {
                    if (!paymentMethodCityMappings.Any())
                        continue;

                    await _DelsamiPaymentMethodService.DeletePaymentMethodCityMappingAsync(paymentMethodCityMappings.FirstOrDefault());
                }
            }
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

        return RedirectToAction("Restrictions");
    }


    public async Task<IActionResult> GetPaymentRestrictions(int stateProvinceId)
    {
        var model = await _DelsamiShippingModelFactory.PrepareDelsamiPaymentMethodRestrictionModelAsync(stateProvinceId);
        return PartialView("~/Plugins/Delsami.Payments.ManagePaymentMethod/Areas/Admin/Views/ManagePaymentMethod/_RestrictionsTable.cshtml", model);
    }

    #endregion


    #region Shipping Method Restriction


    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("SaveShippingMethodRestrictions")]
    public virtual async Task<IActionResult> SaveShippingMethodRestrictions(PaymentMethodShippingRestrictionModel model, IFormCollection form)
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return AccessDeniedView();

        var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();


        var paymentMethods = (await _paymentPluginManager.LoadActivePluginsAsync())
                              .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                              .ToList();

        foreach (var paymentMethod in paymentMethods)
        {
            var paymentMethodModel = paymentMethod.ToPluginModel<PaymentMethodModel>();

            var formKey = "restrict_" + paymentMethodModel.SystemName;
            var shippingMethodIdsToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            foreach (var shippingMethod in shippingMethods)
            {
                var restrict = shippingMethodIdsToRestrict.Contains(shippingMethod.Id);
                var paymentMethodCityMappings = await _paymentMethodShippingRestrictionService.GetPaymentMethodShippingRestrictionMappingAsync(paymentMethodModel.SystemName, shippingMethod.Id);

                if (restrict)
                {
                    if (paymentMethodCityMappings.Any())
                        continue;

                    await _paymentMethodShippingRestrictionService.InsertAsync(new PaymentMethodShippingRestriction { ShippingMethodId = shippingMethod.Id, PaymentMethodSystemName = paymentMethodModel.SystemName });
                }
                else
                {
                    if (!paymentMethodCityMappings.Any())
                        continue;

                    await _paymentMethodShippingRestrictionService.DeleteAsync(paymentMethodCityMappings.FirstOrDefault());
                }
            }
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

        return RedirectToAction("GetShippingMethodRestrictions");
    }


    public virtual async Task<IActionResult> GetShippingMethodRestrictions()
    {
        if (!await _permissionService.AuthorizeAsync(PaymentMethodStateProvinceRestrictionPermission.ManagePaymentMethod))
            return AccessDeniedView();

        //prepare model
        var model = await _DelsamiShippingModelFactory.PreparePaymentMethodShippingRestrictionModelAsync();

        return View("~/Plugins/Delsami.Payments.ManagePaymentMethod/Areas/Admin/Views/ManagePaymentMethod/ShippingMethodRestriction.cshtml", model);
    }

    #endregion


    #region Amount Restriction

    public virtual async Task<IActionResult> AmountRestrictions()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var AvailablePaymentMethods = await _paymentPluginManager.LoadAllPluginsAsync();
        var paymentMethodsRestricted = await _paymentMethodAmountRestrictionService.GetAllAsync();

        var model = new PaymentMethodAmountRestrictionList();
        model.PaymentMethods = AvailablePaymentMethods.Select(a => new PaymentMethodAmountRestrictionModel()
        {
            Id = paymentMethodsRestricted.FirstOrDefault(b => b.PaymentMethodSystemName == a.PluginDescriptor.SystemName)?.Id ?? 0,
            FriendlyName = a.PluginDescriptor.FriendlyName,
            SystemName = a.PluginDescriptor.SystemName,
            MaxAmount = paymentMethodsRestricted.FirstOrDefault(b => b.PaymentMethodSystemName == a.PluginDescriptor.SystemName)?.MaxAmount ?? null,
            MinAmount = paymentMethodsRestricted.FirstOrDefault(b => b.PaymentMethodSystemName == a.PluginDescriptor.SystemName)?.MinAmount ?? null,
        }).ToList();

        return View("~/Plugins/Delsami.Payments.ManagePaymentMethod/Areas/Admin/Views/ManagePaymentMethod/AmountRestrictions.cshtml", model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AmountRestrictionSave(PaymentMethodAmountRestrictionList model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();


        if (!ModelState.IsValid)
        {
            return View("~/Plugins/Delsami.Payments.ManagePaymentMethod/Areas/Admin/Views/ManagePaymentMethod/AmountRestrictions.cshtml", model);
        }

        var entities = model.PaymentMethods.Select(a => new PaymentMethodAmountRestriction()
        {
            Id = a.Id,
            MinAmount = a.MinAmount,
            MaxAmount = a.MaxAmount,
            PaymentMethodSystemName = a.SystemName,
        }).ToList();

        await _paymentMethodAmountRestrictionService.InsertRangeAsync(entities);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

        return RedirectToAction("AmountRestrictions");
    }

    #endregion

    #endregion
}
