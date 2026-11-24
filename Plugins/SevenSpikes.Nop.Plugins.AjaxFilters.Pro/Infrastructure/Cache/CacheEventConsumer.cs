using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;
using SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure.Constants;
using System.Threading.Tasks;

#nullable enable
namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure.Cache
{
  public class CacheEventConsumer :
    IConsumer<
    #nullable disable
    EntityInsertedEvent<Category>>,
    IConsumer<EntityUpdatedEvent<Category>>,
    IConsumer<EntityDeletedEvent<Category>>
  {
    private IStaticCacheManager CacheManager { get; }

    public CacheEventConsumer(IStaticCacheManager cacheManager) => this.CacheManager = cacheManager;

    public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage) => await this.CacheManager.RemoveByPrefixAsync(CacheKeys.AjaxFiltersProCategoriesCachePattern);

    public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage) => await this.CacheManager.RemoveByPrefixAsync(CacheKeys.AjaxFiltersProCategoriesCachePattern);

    public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage) => await this.CacheManager.RemoveByPrefixAsync(CacheKeys.AjaxFiltersProCategoriesCachePattern);
  }
}
