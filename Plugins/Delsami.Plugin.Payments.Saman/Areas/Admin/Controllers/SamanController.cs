using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Payments.Saman.Areas.Admin.Models;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.Saman.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.Admin)]
public class SamanController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public SamanController(ILocalizationService localizationService,INotificationService notificationService,IPermissionService permissionService,
                          ISettingService settingService,IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            return AccessDeniedView();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var SamanSettings = _settingService.LoadSetting<SamanSettings>(storeScope);

        var model = new ConfigurationModel
        {
            BaseUrl = SamanSettings.BaseUrl,
            TimeOutSeconds = SamanSettings.TimeOutSeconds,
            DescriptionText= SamanSettings.DescriptionText,
            TokenCacheMinutes=SamanSettings.TokenCacheMinutes,
            TerminalNumber=SamanSettings.TerminalNumber,
            TerminalPassword=SamanSettings.TerminalPassword,
        };

        return View("~/Plugins/Delsami.Payments.Saman/Areas/Admin/Views/Saman/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var SamanSettings = _settingService.LoadSetting<SamanSettings>(storeScope);

        //save settings
        SamanSettings.BaseUrl = model.BaseUrl;
        SamanSettings.TimeOutSeconds = model.TimeOutSeconds;
        SamanSettings.DescriptionText = model.DescriptionText;
        SamanSettings.TokenCacheMinutes = model.TokenCacheMinutes;
        SamanSettings.TerminalPassword = model.TerminalPassword;
        SamanSettings.TerminalNumber = model.TerminalNumber;


        _settingService.SaveSetting(SamanSettings);
        //now clear settings cache
        _settingService.ClearCache();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }


    #endregion
}
