using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Models;
using Nop.Services.Directory;
using System.Linq;
using System.Collections.Generic;
using ZarIran.Plugin.Shipping.IRPost.Infrastructures.Permissions;
using Nop.Services.Shipping;

namespace ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Controllers;

public class IRPostController : BaseAdminController
{
    #region Fields
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly ICountryService _countryService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IShippingService _shippingService;
    #endregion

    #region Ctor
    public IRPostController(IPermissionService permissionService, ISettingService settingService, ICountryService countryService, IStateProvinceService stateProvinceService, IShippingService shippingService)
    {
        _permissionService = permissionService;
        _settingService = settingService;
        _countryService = countryService;
        _stateProvinceService = stateProvinceService;
        _shippingService = shippingService;
    }
    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
            return AccessDeniedView();

        var settings = await _settingService.LoadSettingAsync<IRPostSettings>();
        var model = await PrepareConfigModelAsync(settings);

        foreach (var sm in await _shippingService.GetAllShippingMethodsAsync())
            model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString() });
        //states
        model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });


        return View("~/Plugins/ZarIran.Shipping.IRPost/Areas/Admin/Views/IRPost/Configure.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
            return Content("Access denied");


        var settings = await _settingService.LoadSettingAsync<IRPostSettings>();

        settings.User=model.User;
        settings.Password=model.Password;
        settings.ContractCode=model.ContractCode;
        settings.PostNodeCode=model.PostNodeCode;
        settings.BaseUrl=model.BaseUrl;
        settings.EnableManualCalc = model.EnableManualCalc;

        settings.NeighbouringProvinces = string.Join(",", model.NeighbouringProvinces);

        //settings.NeighbouringZeroToOneKgPostPrice = model.NeighbouringZeroToOneKgPostPrice;
        //settings.NonNeighbouringZeroToOneKgPostPrice = model.NonNeighbouringZeroToOneKgPostPrice;

        //settings.NeighbouringOneToTwoKgPostPrice = model.NeighbouringOneToTwoKgPostPrice;
        //settings.NonNeighbouringOneToTwoKgPostPrice = model.NonNeighbouringOneToTwoKgPostPrice;

        //settings.NeighbouringTwoToThreeKgPostPrice = model.NeighbouringTwoToThreeKgPostPrice;
        //settings.NonNeighbouringTwoToThreeKgPostPrice = model.NonNeighbouringTwoToThreeKgPostPrice;

        //settings.NeighbouringThreeToFiveKgPostPrice = model.NeighbouringThreeToFiveKgPostPrice;
        //settings.NonNeighbouringThreeToFiveKgPostPrice = model.NonNeighbouringThreeToFiveKgPostPrice;

        //settings.NeighbouringFiveToNineKgPostPrice = model.NeighbouringFiveToNineKgPostPrice;
        //settings.NonNeighbouringFiveToNineKgPostPrice = model.NonNeighbouringFiveToNineKgPostPrice;

        //settings.NeighbouringMoreThanNineKgPostPrice = model.NeighbouringMoreThanNineKgPostPrice;

        settings.PostShippingMethodId = model.ShippingMethodId;


        await _settingService.SaveSettingAsync(settings);
        return RedirectToAction("Configure");
    }


    private async Task<ConfigurationModel> PrepareConfigModelAsync(IRPostSettings settings)
    {
        var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync("IR");
        var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id);

        var statesIds = !string.IsNullOrEmpty(settings?.NeighbouringProvinces)
            ? settings.NeighbouringProvinces.Split(',').Where(id => !string.IsNullOrWhiteSpace(id)).ToList()
            : new List<string>();

        return new ConfigurationModel
        {
            NeighbouringProvinces = statesIds,
            AvailableNeighbouringProvinces = states?.Select(r => new SelectListItem
            {
                Text = r?.Name,
                Value = r?.Id.ToString(),
                Selected = statesIds.Contains(r.Id.ToString())
            }).ToList() ?? new List<SelectListItem>(),

            //NeighbouringZeroToOneKgPostPrice = settings.NeighbouringZeroToOneKgPostPrice,
            //NonNeighbouringZeroToOneKgPostPrice = settings.NonNeighbouringZeroToOneKgPostPrice,

            //NeighbouringOneToTwoKgPostPrice = settings.NeighbouringOneToTwoKgPostPrice,
            //NonNeighbouringOneToTwoKgPostPrice = settings.NonNeighbouringOneToTwoKgPostPrice,

            //NeighbouringTwoToThreeKgPostPrice = settings.NeighbouringTwoToThreeKgPostPrice,
            //NonNeighbouringTwoToThreeKgPostPrice = settings.NonNeighbouringTwoToThreeKgPostPrice,

            //NeighbouringThreeToFiveKgPostPrice = settings.NeighbouringThreeToFiveKgPostPrice,
            //NonNeighbouringThreeToFiveKgPostPrice = settings.NonNeighbouringThreeToFiveKgPostPrice,

            //NeighbouringFiveToNineKgPostPrice = settings.NeighbouringFiveToNineKgPostPrice,
            //NonNeighbouringFiveToNineKgPostPrice = settings.NonNeighbouringFiveToNineKgPostPrice,

           //NeighbouringMoreThanNineKgPostPrice = settings.NeighbouringMoreThanNineKgPostPrice,


            ShippingMethodId = settings.PostShippingMethodId,
            User = settings.User,
            Password = settings.Password,
            ContractCode = settings.ContractCode,
            PostNodeCode = settings.PostNodeCode,
            BaseUrl = settings.BaseUrl,
			EnableManualCalc = settings.EnableManualCalc,
            

        };
    }

    #endregion
}
