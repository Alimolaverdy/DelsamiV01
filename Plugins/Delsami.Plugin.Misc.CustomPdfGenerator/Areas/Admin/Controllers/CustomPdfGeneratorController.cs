using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Delsami.Plugin.Misc.CustomPdfGenerator.Areas.Admin.Models;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Vendors;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZXing;
using System;

namespace Delsami.Plugin.Misc.CustomPdfGenerator.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CustomPdfGeneratorController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public CustomPdfGeneratorController(IPermissionService permissionService,
                                            ISettingService settingService,
                                            ILocalizationService localizationService,
                                            INotificationService notificationService,
                                            IVendorService vendorService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _vendorService = vendorService;
        }

        #endregion

        #region Methods

        public async Task<ActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var customPdfGeneratorSettings = await _settingService.LoadSettingAsync<CustomPdfGeneratorSettings>();
            var vendors = await _vendorService.GetAllVendorsAsync();

            var configurationModel = new ConfigurationModel
            {
                DelsamiVendorId = customPdfGeneratorSettings.DelsamiVendorId,
                BarcodeFormat = customPdfGeneratorSettings.BarcodeFormat,
                PhoneNumber = customPdfGeneratorSettings.PhoneNumber,
                LogoPictureId= customPdfGeneratorSettings.LogoPictureId,
                RotateDegree= customPdfGeneratorSettings.RotateDegree,
                AvailableVendors = vendors.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                    Selected = s.Id == customPdfGeneratorSettings.DelsamiVendorId

                }).ToList(),
                AvailableBarcodeFormats = Enum.GetValues(typeof(BarcodeFormat)).Cast<BarcodeFormat>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList()
            };

            return View("~/Plugins/Delsami.Misc.CustomPdfGenerator/Areas/Admin/Views/CustomPdfGenerator/Configure.cshtml", configurationModel);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var customPdfGeneratorSettings = await _settingService.LoadSettingAsync<CustomPdfGeneratorSettings>();

            if (ModelState.IsValid)
            {

                customPdfGeneratorSettings.DelsamiVendorId = model.DelsamiVendorId;
                customPdfGeneratorSettings.BarcodeFormat = model.BarcodeFormat;
                customPdfGeneratorSettings.PhoneNumber = model.PhoneNumber;
                customPdfGeneratorSettings.LogoPictureId = model.LogoPictureId;
                customPdfGeneratorSettings.RotateDegree = model.RotateDegree;

                _settingService.SaveSetting(customPdfGeneratorSettings);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Configure");
            }

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.Settings.Error"));

            var vendors = await _vendorService.GetAllVendorsAsync();

            var configurationModel = new ConfigurationModel
            {
                DelsamiVendorId = customPdfGeneratorSettings.DelsamiVendorId,
                AvailableVendors = vendors.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(),
                    Selected = s.Id == customPdfGeneratorSettings.DelsamiVendorId

                }).ToList(),
                AvailableBarcodeFormats = Enum.GetValues(typeof(BarcodeFormat)).Cast<BarcodeFormat>().Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList()
            };


            return View("~/Plugins/Delsami.Misc.CustomPdfGenerator/Areas/Admin/Views/CustomPdfGenerator/Configure.cshtml", model);
        }

        #endregion
    }
}
