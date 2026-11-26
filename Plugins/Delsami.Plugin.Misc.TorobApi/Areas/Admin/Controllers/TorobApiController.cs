using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Delsami.Plugin.Misc.TorobApi.Areas.Admin.Models;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.TorobApi.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class TorobApiController : BasePluginController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public TorobApiController(ISettingService settingService,
                                  INotificationService notificationService,
                                  ILocalizationService localizationService)
        {
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            var coreSettings = await _settingService.LoadSettingAsync<TorobApiSettings>();

            var model = new ConfigurationModel
            {
                LogEnabled = coreSettings.LogEnabled,
                TorobPublicKey = coreSettings.TorobPublicKey,
                IsActive = coreSettings.IsActive,
                LogEnabledOrderRequest = coreSettings.LogEnabledOrderRequest,
            };

            return View("~/Plugins/Delsami.Misc.TorobApi/Areas/Admin/Views/TorobApi/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            var torobApiSettings = await _settingService.LoadSettingAsync<TorobApiSettings>();

            torobApiSettings.LogEnabled = model.LogEnabled;
            torobApiSettings.TorobPublicKey = model.TorobPublicKey;
            torobApiSettings.IsActive = model.IsActive;
            torobApiSettings.LogEnabledOrderRequest = model.LogEnabledOrderRequest;

            await _settingService.SaveSettingAsync(torobApiSettings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }

        #endregion
    }
}
