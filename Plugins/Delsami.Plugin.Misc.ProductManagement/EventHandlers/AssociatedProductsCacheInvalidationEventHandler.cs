using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Delsami.Plugin.Misc.ProductManagement.EventHandlers
{
    public class AssociatedProductsCacheInvalidationEventHandler : IConsumer<EntityUpdatedEvent<Product>>,
                                                                   IConsumer<EntityDeletedEvent<Product>>,
                                                                   IConsumer<EntityInsertedEvent<Product>>

    {
        private readonly IStaticCacheManager _staticCacheManager;

        public AssociatedProductsCacheInvalidationEventHandler(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }


        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            await InvalidationCache(eventMessage.Entity);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
        {
            await InvalidationCache(eventMessage.Entity);
        }

        public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
        {
            await InvalidationCache(eventMessage.Entity);
        }

        private Task InvalidationCache(Product product)
        {
            if (product.ProductType == ProductType.SimpleProduct)
                _staticCacheManager.RemoveByPrefix($"DelsamiAssociatedProducts-{product.ParentGroupedProductId}");

            return Task.CompletedTask;
        }
    }
}
