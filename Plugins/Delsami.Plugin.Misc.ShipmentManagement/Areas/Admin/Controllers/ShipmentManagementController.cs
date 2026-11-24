using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Models;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Controllers
{

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class ShipmentManagementController : BasePluginController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ShipmentManagementController(IPermissionService permissionService,
                                                 ISettingService settingService,
                                                 INotificationService notificationService,
                                                 ILocalizationService localizationService)
        {

            _settingService = settingService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var settings = await _settingService.LoadSettingAsync<ShipmentManagementSettings>();

            var model = new ConfigurationModel
            {
                DeliveryCodeToCustomerEnabled = settings.DeliveryCodeToCustomerEnabled,
                DeliveryCodeToCustomerMessageTemplate = settings.DeliveryCodeToCustomerMessageTemplate,
                ShipmentIdIndicatorWordInFile = settings.ShipmentIdIndicatorWordInFile,
                TrackingNumberIndicatorWordInFile = settings.TrackingNumberIndicatorWordInFile,
                IRPostTrackingLink=settings.IRPostTrackingLink,
                TipaxTrackingLink=settings.TipaxTrackingLink,
            };

            return View("~/Plugins/ZarIran.Misc.ShipmentManagement/Areas/Admin/Views/ShipmentManagement/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var shipmentManagementSettings = new ShipmentManagementSettings
            {
                DeliveryCodeToCustomerEnabled = model.DeliveryCodeToCustomerEnabled,
                DeliveryCodeToCustomerMessageTemplate = model.DeliveryCodeToCustomerMessageTemplate,
                TrackingNumberIndicatorWordInFile = model.TrackingNumberIndicatorWordInFile,
                ShipmentIdIndicatorWordInFile=model.ShipmentIdIndicatorWordInFile,
                IRPostTrackingLink = model.IRPostTrackingLink,
                TipaxTrackingLink=model.TipaxTrackingLink,
            };

            await _settingService.SaveSettingAsync(shipmentManagementSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return View("~/Plugins/ZarIran.Misc.ShipmentManagement/Areas/Admin/Views/ShipmentManagement/Configure.cshtml", model);
        }

        #endregion
    }
}
