using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using System;
using System.Threading.Tasks;

namespace SevenSpikes.Nop.Plugins.AjaxCart.Infrastructure.Cache
{
  public class AjaxCartModelCacheEventConsumer : IConsumer<EntityInsertedEvent<Product>>
  {
    private IStaticCacheManager CacheManager { get; set; }

    public AjaxCartModelCacheEventConsumer() => this.CacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();

    public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage) => await this.CacheManager.RemoveByPrefixAsync("nop.pres.7spikes.ajaxcart", Array.Empty<object>());
  }
}
