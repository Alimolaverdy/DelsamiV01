using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Data;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Framework.Plugin;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Components;
using System;
using System.Threading.Tasks;

#nullable enable
namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure
{
    public class AjaxFiltersProPlugin : BaseAdminWidgetPlugin7Spikes
  {
    private readonly
    #nullable disable
    ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    public AjaxFiltersProPlugin(ISettingService settingService, WidgetSettings widgetSettings)
      : base(SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure.Constants.Plugin.MenuItems, "SevenSpikes.Plugins.AjaxFiltersPro.Admin.Menu.MenuName", "SevenSpikes.Nop.Plugins.AjaxFilters.Pro", SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure.Constants.Plugin.IsTrialVersion, "http://www.nop-templates.com/ajax-filters-pro-plugin-for-nopcommerce")
    {
      this._settingService = settingService;
      this._widgetSettings = widgetSettings;
    }

    public override string GetConfigurationPageUrl() => this.StoreLocation + "Admin/AjaxFiltersProAdmin/Settings";

    protected override async Task InstallAdditionalSettingsAsync()
    {
      if (this._widgetSettings.ActiveWidgetSystemNames.Contains("SevenSpikes.Nop.Plugins.AjaxFilters.Pro"))
        return;
      this._widgetSettings.ActiveWidgetSystemNames.Add("SevenSpikes.Nop.Plugins.AjaxFilters.Pro");
      await this._settingService.SaveSettingAsync<WidgetSettings>(this._widgetSettings);
    }

    public override async Task InstallAsync()
    {
      if (DataSettingsManager.LoadSettings().DataProvider == DataProviderType.PostgreSQL)
        throw new NopException("There is no PostgreSQL support in the Ajax Filters plugin");
      await base.InstallAsync();
    }

    public override Type GetWidgetViewComponent(string widgetZone) => typeof (AjaxFiltersProComponent);
  }
}
