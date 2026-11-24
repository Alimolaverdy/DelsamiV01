using SevenSpikes.Nop.Framework.AutoMapper;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Domain.Settings;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Areas.Admin.Extensions
{
  public static class MappingExtensions
  {
    public static NopAjaxFiltersProSettingsModel ToModel(
      this NopAjaxFiltersProSettings nopAjaxFiltersProSettings)
    {
      return nopAjaxFiltersProSettings.MapTo<NopAjaxFiltersProSettings, NopAjaxFiltersProSettingsModel>();
    }

    public static NopAjaxFiltersProSettings ToEntity(
      this NopAjaxFiltersProSettingsModel nopAjaxFiltersProSettingsModel)
    {
      return nopAjaxFiltersProSettingsModel.MapTo<NopAjaxFiltersProSettingsModel, NopAjaxFiltersProSettings>();
    }
  }
}
