using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.TorobPay;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;
using ZarIran.Plugin.Payments.TorobPay.Areas.Admin.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;

namespace ZarIran.Plugin.Payments.TorobPay.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class TorobPayController : BaseAdminController
    {
        #region Fields
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public TorobPayController(IPermissionService permissionService,
                                    ISettingService settingService,
                                    INotificationService notificationService,
                                    ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            
        }

        #endregion

        #region Methods
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var settings = await _settingService.LoadSettingAsync<TorobPaySettings>();
            var model = new ConfigurationModel
            {
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                Password = settings.Password,   
                Username = settings.Username,
                BaseUrl = settings.BaseUrl,
                TokenCacheMinutes = settings.TokenCacheMinutes,
                MaximumAllowedAmount = settings.MaximumAllowedAmount,
                MinimumAllowedAmount = settings.MinimumAllowedAmount,
                TimeoutSeconds = settings.TimeoutSeconds,
            };

            return View("~/Plugins/ZarIran.Payments.TorobPay/Areas/Admin/Views/TorobPay/Configure.cshtml", model);
        }


        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var settings = await _settingService.LoadSettingAsync<TorobPaySettings>();

            settings.ClientId = model.ClientId;
            settings.ClientSecret = model.ClientSecret;
            settings.Username = model.Username;
            settings.Password = model.Password;
            settings.BaseUrl = model.BaseUrl;
            settings.TokenCacheMinutes = model.TokenCacheMinutes;
            settings.MinimumAllowedAmount = model.MinimumAllowedAmount;
            settings.MaximumAllowedAmount = model.MaximumAllowedAmount;
            settings.TimeoutSeconds = model.TimeoutSeconds;

            await _settingService.SaveSettingAsync(settings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Configure");
        }
    }
    #endregion
}
