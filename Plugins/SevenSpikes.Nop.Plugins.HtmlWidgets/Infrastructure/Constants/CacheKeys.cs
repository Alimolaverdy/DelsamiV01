using Nop.Core.Caching;
using SevenSpikes.Nop.Conditions.Domain;
using SevenSpikes.Nop.Mappings.Domain;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure.Constants;

public static class CacheKeys
{
    public static CacheKey HtmlWidgetsWidgetZonesDictionaryCacheKey => new CacheKey("SevenSpikes.Nop.Plugins.HtmlWidgets.WidgetZonesMappingDictionary", new string[1] { NopEntityCacheDefaults<EntityWidgetMapping>.AllPrefix });

    public static CacheKey ProductHtmlWidgetsForWidgetCacheKey => new CacheKey("SevenSpikes.Nop.Plugins.HtmlWidgets.{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", new string[7]
    {
        NopEntityCacheDefaults<EntityWidgetMapping>.AllPrefix,
        NopEntityCacheDefaults<Condition>.AllPrefix,
        NopEntityCacheDefaults<ConditionGroup>.AllPrefix,
        NopEntityCacheDefaults<ConditionStatement>.AllPrefix,
        NopEntityCacheDefaults<EntityCondition>.AllPrefix,
        NopEntityCacheDefaults<CustomerOverride>.AllPrefix,
        NopEntityCacheDefaults<ProductOverride>.AllPrefix
    });
}
