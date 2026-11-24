using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Enums;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Areas.Admin.Models
{
  public class NopAjaxFiltersProSettingsModel
  {
    public int ActiveStoreScopeConfiguration { get; set; }

    public bool IsTrialVersion { get; set; }

    [NopResourceDisplayName("SevenSpikes.NopAjaxFiltersPro.Admin.Settings.EnableAjaxFiltersPro")]
    public bool EnableAjaxFiltersPro { get; set; }

    public bool EnableAjaxFiltersPro_OverrideForStore { get; set; }

    [NopResourceDisplayName("SevenSpikes.NopAjaxFiltersPro.Admin.Settings.WidgetZone")]
    public string WidgetZone { get; set; }

    public bool WidgetZone_OverrideForStore { get; set; }

    [NopResourceDisplayName("SevenSpikes.NopAjaxFiltersPro.Admin.Settings.UIMode")]
    public NopAjaxFiltersProUIMode UIMode { get; set; }

    public SelectList AvailableUiModes { get; set; }

    public bool UIMode_OverrideForStore { get; set; }

    [NopResourceDisplayName("SevenSpikes.NopAjaxFiltersPro.Admin.Settings.ShowTopLevelCategoriesOnly")]
    public bool ShowTopLevelCategoriesOnly { get; set; }

    public bool ShowTopLevelCategoriesOnly_OverrideForStore { get; set; }

    public SelectList SupportedWidgetZones { get; set; }
  }
}
