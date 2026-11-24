using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Models;
using Nop.Plugin.Misc.AdvRedirect.Infrastructure.Permissions;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Areas.Admin.Controllers
{
    public class AdvRedirectController : BaseAdminController
    {
        #region  Fields

        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public AdvRedirectController(ISettingService settingService, ILocalizationService localizationService,
            IPermissionService permissionService, INotificationService notificationService)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods


        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(AdvRedirectPermission.ManageRedirectionRule))
                return Content("Access denied");


            var advRedirectSetting = await _settingService.LoadSettingAsync<AdvRedirectSettings>();

            ConfigurationModel model = new();

            model.EnableAdvRedirectHistoryLogging = advRedirectSetting.EnableAdvRedirectHistoryLogging;

            return View($"~/Plugins/Misc.AdvRedirect/Areas/Admin/Views/AdvRedirect/Configure.cshtml", model);

        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            var advRedirectSetting = await _settingService.LoadSettingAsync<AdvRedirectSettings>();

            advRedirectSetting.EnableAdvRedirectHistoryLogging = model.EnableAdvRedirectHistoryLogging;

            await _settingService.SaveSettingAsync(advRedirectSetting);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion

    }
}
