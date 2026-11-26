using System.Threading.Tasks;
using Delsami.Plugin.Misc.ProductManagement.Infrastructure.Cache;
using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Delsami.Plugin.Misc.ProductManagement.EventHandlers
{
    public class VirtualProductsCacheInvalidationEventHandler : CacheEventConsumer<Product>
	{
		protected override Task ClearCacheAsync(Product entity)
		{
			if (entity.ProductType == ProductType.SimpleProduct)
				_staticCacheManager.RemoveByPrefix(DelsamiProductCacheDefaults.AllVirtualProducts);

			return Task.CompletedTask;
		}

	}
}
