using Nop.Core.Caching;

namespace Delsami.Plugin.Misc.ProductManagement.Infrastructure.Cache
{
    public static partial class DelsamiProductCacheDefaults
    {
        public static string VirtualProductsByMainProductIdPrefixCacheKey => "VirtualProductsByMainProductId.{0}";
        public static string AllVirtualProducts => "AllVirtualProducts";
        public static CacheKey VirtualProductsByMainProductIdCacheKey => new("Delsami.Plugin.Misc.ProductManagement-VirtualProductsByMainProductId-{0}",
                                                                              VirtualProductsByMainProductIdPrefixCacheKey,
                                                                              AllVirtualProducts);
    }
}
