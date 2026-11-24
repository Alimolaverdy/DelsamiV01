using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Cms;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Conditions.Services;
using SevenSpikes.Nop.Framework.Plugin;
using SevenSpikes.Nop.Mappings.Services;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Components;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure.Constants;
using SevenSpikes.Nop.Scheduling.Services;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure;

public class HtmlWidgetsPlugin : BaseAdminWidgetPlugin7Spikes
{
    private readonly IConditionsInstallerService _conditionsInstallerService;

    private readonly IMappingInstallerService _mappingInstallerService;

    private readonly ISchedulingInstallerService _schedulingInstallerService;

    private readonly ISettingService _settingService;

    private readonly WidgetSettings _widgetSettings;

    private static readonly List<MenuItem7Spikes> MenuItems;

    public static bool IsTrialVersion => false;

    public HtmlWidgetsPlugin(IConditionsInstallerService conditionsInstallerService, IMappingInstallerService mappingInstallerService, ISchedulingInstallerService schedulingInstallerService, ISettingService settingService, WidgetSettings widgetSettings)
        : base(MenuItems, "SevenSpikes.Plugins.HtmlWidgets.Admin.Menu.MenuName", "SevenSpikes.Nop.Plugins.HtmlWidgets", IsTrialVersion, "http://www.nop-templates.com/html-widgets-plugin-for-nopcommerce")
    {
        _conditionsInstallerService = conditionsInstallerService;
        _mappingInstallerService = mappingInstallerService;
        _schedulingInstallerService = schedulingInstallerService;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    public override string GetConfigurationPageUrl()
    {
        return StoreLocation + "Admin/HtmlWidgetsPluginAdmin/Settings";
    }

    protected override async Task InstallAdditionalSettingsAsync()
    {
        await _conditionsInstallerService.InstallConditionsForEntityAsync(Plugin.EntityType);
        await _mappingInstallerService.InstallMappingsForEntityAsync(Plugin.EntityType);
        await _schedulingInstallerService.InstallSchedulingForEntityAsync(Plugin.EntityType);
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.HtmlWidgets"))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add("SevenSpikes.Nop.Plugins.HtmlWidgets");
            await _settingService.SaveSettingAsync<WidgetSettings>(_widgetSettings, 0);
        }
    }

    protected override async Task UninstallAdditionalSettingsAsync()
    {
        await _conditionsInstallerService.UnInstallConditionsForEntityAsync(Plugin.EntityType);
        await _mappingInstallerService.UnInstallMappingsForEntityAsync(Plugin.EntityType);
        await _schedulingInstallerService.UnInstallSchedulingForEntityAsync(Plugin.EntityType);
    }

    public override Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(HtmlWidgetComponent);
    }

    static HtmlWidgetsPlugin()
    {
        List<MenuItem7Spikes> list = new List<MenuItem7Spikes>();
        MenuItem7Spikes item = new();
        item.SubMenuName = "SevenSpikes.Plugins.HtmlWidgets.Admin.Submenus.Settings";
        item.SubMenuRelativePath = "HtmlWidgetsPluginAdmin/Settings";
        list.Add(item);
        item = new();
        item.SubMenuName = "SevenSpikes.Plugins.HtmlWidgets.Admin.Submenus.ManageHtmlWidgets";
        item.SubMenuRelativePath = "HtmlWidgetAdmin/List";
        list.Add(item);
        MenuItems = list;
    }
}
