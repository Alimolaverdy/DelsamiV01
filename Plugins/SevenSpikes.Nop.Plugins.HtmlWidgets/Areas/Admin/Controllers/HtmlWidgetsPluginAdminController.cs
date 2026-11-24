using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Cms;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using SevenSpikes.Nop.Framework.Areas.Admin.ControllerAttributes;
using SevenSpikes.Nop.Framework.Areas.Admin.Controllers;
using SevenSpikes.Nop.Framework.Areas.Admin.Helpers;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Extensions;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Controllers;

[ManagePluginsAdminAuthorize("SevenSpikes.Nop.Plugins.HtmlWidgets", false)]
public class HtmlWidgetsPluginAdminController : Base7SpikesAdminController
{
    private const string PluginSystemName = "SevenSpikes.Nop.Plugins.HtmlWidgets";

    private readonly WidgetSettings _widgetSettings;
    private readonly ISettingService _settingService;
    private readonly ICustomerActivityService _customerActivityService;

    public HtmlWidgetsPluginAdminController(
        ISettingService settingService,
        ICustomerActivityService customerActivityService,
        WidgetSettings widgetSettings)
    {
        _settingService = settingService;
        _customerActivityService = customerActivityService;
        _widgetSettings = widgetSettings;
    }

    public async Task<ActionResult> Settings()
    {
        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<HtmlWidgetsSettings>(storeScope);
        var model = settings.ToModel();

        model.ActiveStoreScopeConfiguration = storeScope;
        model.EnableHtmlWidgets = model.EnableHtmlWidgets
            && _widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName);

        if (storeScope > 0)
        {
            var storeSettings = new StoreScopeSettingsHelper<HtmlWidgetsSettings>(
                settings,
                storeScope,
                _settingService);

            model.EnableHtmlWidgets_OverrideForStore =
                await storeSettings.SettingExistsAsync(setting => setting.EnableHtmlWidgets);
        }

        model.IsTrialVersion = false;
        return View(nameof(Settings), model);
    }

    [HttpPost]
    public async Task<ActionResult> Settings(HtmlWidgetsSettingsModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Settings));

        if (model.EnableHtmlWidgets && !_widgetSettings.ActiveWidgetSystemNames.Contains(PluginSystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(PluginSystemName);
            await _settingService.SaveSettingAsync(_widgetSettings, 0);
        }

        var storeScope = await StoreContext.GetActiveStoreScopeConfigurationAsync();
        var storeSettings = new StoreScopeSettingsHelper<HtmlWidgetsSettings>(
            model.ToEntity(),
            storeScope,
            _settingService);

        await storeSettings.SaveStoreSettingAsync(
            model.EnableHtmlWidgets_OverrideForStore,
            setting => setting.EnableHtmlWidgets);
        await _settingService.ClearCacheAsync();
        await _customerActivityService.InsertActivityAsync(
            "EditHtmlWidgetsSettings",
            "Edit Nop Html Widgets Settings");

        SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Configuration.Updated"));
        return RedirectToAction(nameof(Settings));
    }
}
