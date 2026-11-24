using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Cms;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Framework.Plugin;
using SevenSpikes.Nop.Mappings.Services;
using SevenSpikes.Nop.Plugins.JCarousel.Components;
using SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Constants;

namespace SevenSpikes.Nop.Plugins.JCarousel.Infrastructure;

public class JCarouselPlugin : BaseAdminWidgetPlugin7Spikes
{
    private readonly IMappingInstallerService _mappingInstallerService;

    private readonly WidgetSettings _widgetSettings;

    private readonly ISettingService _settingService;

    private static readonly List<MenuItem7Spikes> MenuItems = new List<MenuItem7Spikes>
    {
        new MenuItem7Spikes
        {
            SubMenuName = "SevenSpikes.JCarousel.Admin.Submenus.Settings",
            SubMenuRelativePath = "JCarouselAdmin/Settings"
        },
        new MenuItem7Spikes
        {
            SubMenuName = "SevenSpikes.JCarousel.Admin.Submenus.ManageSettings",
            SubMenuRelativePath = "JCarouselAdmin/List"
        }
    };

    private static bool IsTrialVersion => false;

    public JCarouselPlugin(IMappingInstallerService mappingInstallerService, WidgetSettings widgetSettings, ISettingService settingService)
        : base(MenuItems, "SevenSpikes.Plugins.JCarousel.Admin.Menu.MenuName", "SevenSpikes.Nop.Plugins.JCarousel", IsTrialVersion, "http://www.nop-templates.com/jcarousel-plugin-for-nopcommerce")
    {
        _mappingInstallerService = mappingInstallerService;
        _widgetSettings = widgetSettings;
        _settingService = settingService;
    }

    public override string GetConfigurationPageUrl()
    {
        return base.StoreLocation + "Admin/JCarouselAdmin/Settings";
    }

    protected override async Task InstallAdditionalSettingsAsync()
    {
        await _mappingInstallerService.InstallMappingsForEntityAsync(Plugin.EntityType);
        if (!_widgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.JCarousel"))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add("SevenSpikes.Nop.Plugins.JCarousel");
            await _settingService.SaveSettingAsync<WidgetSettings>(_widgetSettings, 0);
        }
    }

    protected override async Task UninstallAdditionalSettingsAsync()
    {
        await _mappingInstallerService.UnInstallMappingsForEntityAsync(Plugin.EntityType);
    }

    public override Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(JCarouselComponent);
    }
}
