using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Framework.Plugin;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Constants;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure;

public class AjaxFiltersPlugin : BaseAdminWidgetPlugin7Spikes
{
    private readonly WidgetSettings _widgetSettings;
    private readonly ISettingService _settingService;
    private readonly IAjaxFiltersDatabaseService _ajaxFiltersDatabaseService;

    public AjaxFiltersPlugin(
        WidgetSettings widgetSettings,
        ISettingService settingService,
        IAjaxFiltersDatabaseService ajaxFiltersDatabaseService)
        : base(
            Plugin.MenuItems,
            "SevenSpikes.Plugins.AjaxFilters.Admin.Menu.MenuName",
            "SevenSpikes.Nop.Plugins.AjaxFilters",
            Plugin.IsTrialVersion,
            "http://www.nop-templates.com/ajax-filters-plugin-for-nopcommerce")
    {
        _widgetSettings = widgetSettings;
        _settingService = settingService;
        _ajaxFiltersDatabaseService = ajaxFiltersDatabaseService;
    }

    public override string GetConfigurationPageUrl()
    {
        return StoreLocation + "Admin/NopAjaxFiltersAdmin/Settings";
    }

    protected override async Task InstallAdditionalSettingsAsync()
    {
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.AjaxFilters"))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add("SevenSpikes.Nop.Plugins.AjaxFilters");
            await _settingService.SaveSettingAsync<WidgetSettings>(_widgetSettings, 0);
        }

        await _ajaxFiltersDatabaseService.RemoveDatabaseScriptsAsync();
        await _ajaxFiltersDatabaseService.CreateDatabaseScriptsAsync();
    }

    protected override async Task UninstallAdditionalSettingsAsync()
    {
        var settings = EngineContext.Current.Resolve<NopAjaxFiltersSettings>();
        settings.WidgetZone = string.Empty;
        await _settingService.SaveSettingAsync<NopAjaxFiltersSettings>(settings, 0);
        await _ajaxFiltersDatabaseService.RemoveDatabaseScriptsAsync();
    }

    public override async Task InstallAsync()
    {
        if ((int)DataSettingsManager.LoadSettings((INopFileProvider)null, false).DataProvider == 3)
            throw new NopException("There is no PostgreSQL support in the Ajax Filters plugin");

        await ((BasePlugin7Spikes)this).InstallAsync();
    }

    public override Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(NopAjaxFiltersComponent);
    }
}
