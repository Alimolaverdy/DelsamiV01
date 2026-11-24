using System;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using SevenSpikes.Nop.Mappings.Domain;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;

namespace SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Cache;

public class JCarouselModelCacheEventConsumer : IConsumer<EntityInsertedEvent<Product>>, IConsumer<EntityUpdatedEvent<Product>>, IConsumer<EntityDeletedEvent<Product>>, IConsumer<EntityInsertedEvent<Manufacturer>>, IConsumer<EntityUpdatedEvent<Manufacturer>>, IConsumer<EntityDeletedEvent<Manufacturer>>, IConsumer<EntityInsertedEvent<Vendor>>, IConsumer<EntityUpdatedEvent<Vendor>>, IConsumer<EntityDeletedEvent<Vendor>>, IConsumer<EntityUpdatedEvent<Category>>, IConsumer<EntityDeletedEvent<Category>>, IConsumer<EntityInsertedEvent<Category>>, IConsumer<EntityInsertedEvent<Carousel>>, IConsumer<EntityDeletedEvent<Carousel>>, IConsumer<EntityUpdatedEvent<Carousel>>, IConsumer<EntityInsertedEvent<EntityMapping>>, IConsumer<EntityDeletedEvent<EntityMapping>>, IConsumer<EntityUpdatedEvent<EntityMapping>>, IConsumer<EntityInsertedEvent<EntityWidgetMapping>>, IConsumer<EntityDeletedEvent<EntityWidgetMapping>>, IConsumer<EntityUpdatedEvent<EntityWidgetMapping>>
{
    private IStaticCacheManager _staticCacheManager;

    private IStaticCacheManager StaticCacheManager
    {
        get
        {
            if (_staticCacheManager == null)
            {
                _staticCacheManager = EngineContext.Current.Resolve<IStaticCacheManager>();
            }
            return _staticCacheManager;
        }
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Carousel> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Carousel> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Carousel> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityInsertedEvent<EntityMapping> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityDeletedEvent<EntityMapping> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<EntityMapping> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityInsertedEvent<EntityWidgetMapping> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityDeletedEvent<EntityWidgetMapping> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<EntityWidgetMapping> eventMessage)
    {
        await StaticCacheManager.RemoveByPrefixAsync("nop.pres.jcarousel.", Array.Empty<object>());
    }
}
