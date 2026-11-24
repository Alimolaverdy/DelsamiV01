using Nop.Core.Configuration;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Enums;
using System.Collections.Generic;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Settings
{
  public class NopAjaxFiltersProSettings : ISettings
  {
    public NopAjaxFiltersProSettings() => this.ActiveSpecificationAttributes = new List<string>();

    public bool EnableAjaxFiltersPro { get; set; }

    public string WidgetZone { get; set; }

    public bool ShowTopLevelCategoriesOnly { get; set; }

    public NopAjaxFiltersProUIMode UIMode { get; set; }

    public List<string> ActiveSpecificationAttributes { get; set; }
  }
}
