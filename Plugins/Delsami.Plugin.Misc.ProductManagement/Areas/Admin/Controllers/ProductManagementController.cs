using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.Configurations;
using Delsami.Plugin.Misc.ProductManagement.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
[AutoValidateAntiforgeryToken]
public class ProductManagementController : BasePluginController
{
    #region Fields

    private readonly ISettingService _settingService;
    private readonly IPermissionService _permissionService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly ICategoryService _categoryService;

    #endregion

    #region Ctor

    public ProductManagementController(ISettingService settingService,
                                      IPermissionService permissionService,
                                      INotificationService notificationService,
                                      ILocalizationService localizationService,
                                      ICategoryService categoryService)
    {
        _settingService = settingService;
        _permissionService = permissionService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _categoryService = categoryService;
    }

    #endregion

    #region Methods

    public async Task<ActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return Content("Access denied");

        var productManagementSettings = await _settingService.LoadSettingAsync<ProductManagementSettings>();

        var categories = await _categoryService.GetAllCategoriesAsync(showHidden:true);

        var configurationModel = new ConfigurationModel
        {
            GroupedProductOrderBy = productManagementSettings.GroupedProductOrderBy,
            SortType = productManagementSettings.SortType,
            BundleProductCategoryId = productManagementSettings.BundleProductCategoryId
        };

        configurationModel.AvailableCategories = categories.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString(),
            Selected = s.Id == productManagementSettings.BundleProductCategoryId
        }).ToList();

        return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/ProductManagement/Configure.cshtml", configurationModel);
    }



    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return Content("Access denied");

        if (ModelState.IsValid)
        {
            var productManagementSettings = await _settingService.LoadSettingAsync<ProductManagementSettings>();

            productManagementSettings.GroupedProductOrderBy = model.GroupedProductOrderBy;
            productManagementSettings.SortType = model.SortType;
            productManagementSettings.BundleProductCategoryId = model.BundleProductCategoryId;

            _settingService.SaveSetting(productManagementSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Settings.Error"));
        return View("~/Plugins/Delsami.Misc.ProductManagement/Areas/Admin/Views/ProductManagement/Configure.cshtml", model);
    }

    //public async Task<IActionResult> Initialize()
    //{
    //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
    //        return Content("Access denied");

    //    var extendedVendorSettings = await _settingService.LoadSettingAsync<ProductManagementSettings>();

    //    if (!extendedVendorSettings.Initialized)
    //    {
    //        await _warehouseAddressCustomerDistanceService.InitializeWarehouseAddressCustomerDistancesAsync();

    //        extendedVendorSettings.Initialized = true;

    //        await _settingService.SaveSettingAsync(extendedVendorSettings);
    //    }

    //    return RedirectToAction("Configure");
    //}

    #endregion

}
