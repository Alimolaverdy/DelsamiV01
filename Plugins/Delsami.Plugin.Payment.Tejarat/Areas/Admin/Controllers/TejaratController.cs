using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Payment.Tejarat.Areas.Admin.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payment.Tejarat.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class TejaratController : BasePluginController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        //private readonly IValidator<ConfigurationModel> _validator;

        #endregion

        #region Ctor

        public TejaratController(IPermissionService permissionService,
                                    ISettingService settingService,
                                    INotificationService notificationService,
                                    ILocalizationService localizationService
                                    /*IValidator<ConfigurationModel> validator*/)
        {

            _settingService = settingService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            //_validator = validator;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            var settings = await _settingService.LoadSettingAsync<TejaratSettings>();

            var model = new ConfigurationModel
            {
                AcceptorId = settings.AcceptorId,
                PassPhrase = settings.PassPhrase,
                RsaPublicKey = settings.RsaPublicKey,
                TerminalId = settings.TerminalId,
            };

            return View("~/Plugins/Delsami.Payment.Tejarat/Areas/Admin/Views/Tejarat/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return await AccessDeniedDataTablesJson();

            //ValidationResult result = _validator.Validate(model);

            //if (!result.IsValid)
            //{
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.ErrorMessage);
            //    }
            //}

            if (ModelState.IsValid)
            {
                var settings = new TejaratSettings
                {
                    AcceptorId = model.AcceptorId,
                    PassPhrase = model.PassPhrase,
                    RsaPublicKey = model.RsaPublicKey,
                    TerminalId = model.TerminalId,
                };

                await _settingService.SaveSettingAsync(settings);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Configure", model);
            }
            return View("~/Plugins/Delsami.Payment.Tejarat/Areas/Admin/Views/Tejarat/Configure.cshtml", model);

        }
        #endregion
    }
}
