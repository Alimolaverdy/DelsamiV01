using Nop.Core.Caching;

namespace SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Constants;

public static class CacheKeys
{
    public const string JCAROUSEL_KEY = "nop.pres.jcarousel.";

    public const string JCAROUSEL_WIDGETS_KEY = "nop.pres.jcarousel.widgets.";

    public static CacheKey CarouselWidgetByWidgetZoneKey => new CacheKey("nop.pres.jcarousel.widgets.-{0}-{1}", new[] { "nop.pres.jcarousel." });

    public static CacheKey CarouselModelCacheKey => new CacheKey("nop.pres.jcarousel.widgets.-{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}-{9}-{10}", new[] { "nop.pres.jcarousel." });
}
