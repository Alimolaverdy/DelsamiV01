using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Factories;
using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Models;
using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;
using Delsami.Plugin.Shipping.ManageShippingMethod.Domain;
using Delsami.Plugin.Shipping.ManageShippingMethod.Infrastructures.Permissions;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Services.CityService;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Controllers;

public class ManageShippingMethodController : BaseAdminController
{
    #region Fields
    private readonly IPermissionService _permissionService;
    private readonly IDelsamiShippingModelFactory _DelsamiShippingModelFactory;
    private readonly IShippingService _shippingService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IDelsamiStateProvinceService _DelsamiStateProvinceService;
    private readonly IDelsamiShippingService _DelsamiShippingService;
    private readonly ISettingService _settingService;
    private readonly ICityService _cityService;
    #endregion

    #region Ctor

    public ManageShippingMethodController(IPermissionService permissionService,
                                          IDelsamiShippingModelFactory DelsamiShippingModelFactory,
                                          IShippingService shippingService,
                                          INotificationService notificationService,
                                          ILocalizationService localizationService,
                                          IDelsamiStateProvinceService DelsamiStateProvinceService,
                                          IDelsamiShippingService DelsamiShippingService,
                                          ICityService cityService,
                                          ISettingService settingService)
    {
        _permissionService = permissionService;
        _DelsamiShippingModelFactory = DelsamiShippingModelFactory;
        _shippingService = shippingService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _DelsamiStateProvinceService = DelsamiStateProvinceService;
        _DelsamiShippingService = DelsamiShippingService;
        _settingService = settingService;
        _cityService = cityService;
    }
    #endregion

    #region Methods
    public async Task<ActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(ShippingMethodStateProvinceRestrictionPermission.ManageShippingMethod))
            return Content("Access denied");

        var setting = await _settingService.LoadSettingAsync<ManageShippingMethodSetting>();
        var selectedCountryId = setting.CountryId;


        var model = await _DelsamiShippingModelFactory.PrepareConfigureModelAsync(new ConfigurationModel { CountryId = selectedCountryId });
        model.DisabledShippingMethods = !string.IsNullOrWhiteSpace(setting.DisabledShippingMethods) ? setting.DisabledShippingMethods.Split(',').Select(int.Parse)?.ToList() : new List<int>();
        model.AvailableShippingMethods = await prepareShippingMethods();
        model.FulfillmentDecisionEnabled = setting.FulfillmentDecisionEnabled;
        model.FulfillmentDecisionBaseDomain = setting.FulfillmentDecisionBaseDomain;

        return View("~/Plugins/Delsami.Shipping.ManageShippingMethod/Areas/Admin/Views/ManageShippingMethod/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(ShippingMethodStateProvinceRestrictionPermission.ManageShippingMethod))
            return Content("Access denied");

        var settings = await _settingService.LoadSettingAsync<ManageShippingMethodSetting>();
        settings.CountryId = model.CountryId;
        settings.FulfillmentDecisionEnabled = model.FulfillmentDecisionEnabled;
        settings.FulfillmentDecisionBaseDomain = model.FulfillmentDecisionBaseDomain;
        settings.DisabledShippingMethods = model.DisabledShippingMethods != null ? string.Join(",", model.DisabledShippingMethods) : "";

        await _settingService.SaveSettingAsync(settings);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

        return RedirectToAction("Configure");
    }




    public virtual async Task<IActionResult> Restrictions()
    {
        if (!await _permissionService.AuthorizeAsync(ShippingMethodStateProvinceRestrictionPermission.ManageShippingMethod))
            return AccessDeniedView();

        //prepare model
        var model = await _DelsamiShippingModelFactory.PrepareDelsamiShippingMethodRestrictionModelAsync(new DelsamiShippingMethodStateProvinceRestrictionModel());

        return View("~/Plugins/Delsami.Shipping.ManageShippingMethod/Areas/Admin/Views/ManageShippingMethod/Restrictions.cshtml", model);
    }

    public async Task<IActionResult> GetShippingRestrictions(int stateProvinceId)
    {
        var model = await _DelsamiShippingModelFactory.PrepareDelsamiShippingMethodRestrictionModelAsync(stateProvinceId);

        return PartialView("~/Plugins/Delsami.Shipping.ManageShippingMethod/Areas/Admin/Views/ManageShippingMethod/_RestrictionsTable.cshtml", model);

    }


    //we ignore this filter for increase RequestFormLimits
    [IgnoreAntiforgeryToken]
    //we use 2048 value because in some cases default value (1024) is too small for this action
    [RequestFormLimits(ValueCountLimit = 2048)]
    [HttpPost, ActionName("Restrictions")]
    public virtual async Task<IActionResult> RestrictionSave(DelsamiShippingMethodStateProvinceRestrictionModel model, IFormCollection form)
    {
        if (!await _permissionService.AuthorizeAsync(ShippingMethodStateProvinceRestrictionPermission.ManageShippingMethod))
            return AccessDeniedView();

        var cities = await _cityService.GetAllByProvinceId(model.SelectedStateProviceId);

        var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();

        foreach (var shippingMethod in shippingMethods)
        {
            var formKey = "restrict_" + shippingMethod.Id;
            var CityIdsToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            foreach (var city in cities)
            {
                var restrict = CityIdsToRestrict.Contains(city.Id);
                var shippingMethodCityMappings = await _DelsamiShippingService.GetShippingMethodCityMappingAsync(shippingMethod.Id, city.Id);

                if (restrict)
                {
                    if (shippingMethodCityMappings.Any())
                        continue;

                    await _DelsamiShippingService.InsertShippingMethodCityMappingAsync(new ShippingMethodCityRestriction { CityId = city.Id, ShippingMethodId = shippingMethod.Id });
                    await _shippingService.UpdateShippingMethodAsync(shippingMethod);
                }
                else
                {
                    if (!shippingMethodCityMappings.Any())
                        continue;

                    await _DelsamiShippingService.DeleteShippingMethodCityMappingAsync(shippingMethodCityMappings.FirstOrDefault());
                    await _shippingService.UpdateShippingMethodAsync(shippingMethod);
                }
            }
        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Restrictions.Updated"));

        return RedirectToAction("Restrictions");
    }



    #endregion

    #region Internal Methods
    private async Task<IList<SelectListItem>> prepareShippingMethods()
    {
        var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();
        return shippingMethods.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString()
        }).ToList();
    }
    #endregion
}
