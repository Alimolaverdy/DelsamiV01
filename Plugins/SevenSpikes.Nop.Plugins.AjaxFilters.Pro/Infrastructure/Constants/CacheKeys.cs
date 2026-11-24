using Nop.Core.Caching;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure.Constants
{
  public static class CacheKeys
  {
    public static readonly string AjaxFiltersProCategoriesCachePattern = "nop.pres.nop.ajax.filters.pro.categories.";

    public static CacheKey AjaxFiltersProCategoriesCacheKey => new CacheKey(CacheKeys.AjaxFiltersProCategoriesCachePattern + "{0}-{1}-{2}-{3}", new string[1]
    {
      CacheKeys.AjaxFiltersProCategoriesCachePattern
    });
  }
}
