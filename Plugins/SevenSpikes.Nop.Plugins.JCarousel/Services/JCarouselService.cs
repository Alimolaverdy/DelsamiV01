using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Data;
using SevenSpikes.Nop.EntitySettings.Services.EntitySettings;
using SevenSpikes.Nop.Mappings.Domain;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;
using SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Constants;

namespace SevenSpikes.Nop.Plugins.JCarousel.Services;

public class JCarouselService : IJCarouselService
{
    private readonly IEntitySettingService _entitySettingService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IRepository<Carousel> _carouselRepository;
    private readonly IRepository<EntityWidgetMapping> _entityWidgetMappingRepository;

    public JCarouselService(
        IEntitySettingService entitySettingService,
        IEventPublisher eventPublisher,
        IRepository<Carousel> carouselRepository,
        IRepository<EntityWidgetMapping> entityWidgetMappingRepository)
    {
        _entitySettingService = entitySettingService;
        _eventPublisher = eventPublisher;
        _carouselRepository = carouselRepository;
        _entityWidgetMappingRepository = entityWidgetMappingRepository;
    }

    public async Task InsertCarouselAsync(Carousel carousel)
    {
        if (carousel == null)
            throw new ArgumentNullException(nameof(carousel));

        await _carouselRepository.InsertAsync(carousel, true);
        await _entitySettingService.SaveSettingsAsync(
            carousel.JCarouselEntitySettings,
            Plugin.EntityType,
            carousel.Id);
        await _eventPublisher.EntityInsertedAsync(carousel);
    }

    public async Task UpdateCarouselAsync(Carousel carousel)
    {
        if (carousel == null)
            throw new ArgumentNullException(nameof(carousel));

        await _carouselRepository.UpdateAsync(carousel, true);
        await _entitySettingService.SaveSettingsAsync(
            carousel.JCarouselEntitySettings,
            Plugin.EntityType,
            carousel.Id);
        await _eventPublisher.EntityUpdatedAsync(carousel);
    }

    public async Task DeleteCarouselAsync(Carousel carousel)
    {
        if (carousel == null)
            throw new ArgumentNullException(nameof(carousel));

        await _carouselRepository.DeleteAsync(carousel, true);
        await _entitySettingService.DeleteSettingsAsync<JCarouselEntitySettings>(
            Plugin.EntityType,
            carousel.Id);
        await _eventPublisher.EntityDeletedAsync(carousel);
    }

    public async Task<IList<Carousel>> GetAllCarouselsAsync()
    {
        return await AsyncIQueryableExtensions.ToListAsync(_carouselRepository.Table);
    }

    public Task<Carousel> GetCarouselByNameAsync(string name)
    {
        return AsyncIQueryableExtensions.FirstOrDefaultAsync(
            _carouselRepository.Table.Where(carousel => carousel.Name == name),
            (System.Linq.Expressions.Expression<Func<Carousel, bool>>)null);
    }

    public async Task<Carousel> GetCarouselByIdAsync(int carouselId)
    {
        if (carouselId == 0)
            return null;

        var carousel = await _carouselRepository.GetByIdAsync(
            (int?)carouselId,
            (Func<IStaticCacheManager, CacheKey>)null,
            true);

        if (carousel == null)
            return null;

        carousel.JCarouselEntitySettings = await _entitySettingService.LoadEntitySettingsAsync<JCarouselEntitySettings>(
            Plugin.EntityType,
            carousel.Id);

        return carousel;
    }

    public async Task<IList<Carousel>> GetCarouselsByWidgetZoneAsync(string widgetZone)
    {
        var query = from carousel in _carouselRepository.Table
                    join mapping in _entityWidgetMappingRepository.Table
                        on carousel.Id equals mapping.EntityId
                    where (int)mapping.EntityType == (int)Plugin.EntityType
                          && mapping.WidgetZone == widgetZone
                    orderby mapping.DisplayOrder
                    select carousel;

        var carousels = await AsyncIQueryableExtensions.ToListAsync(query);
        foreach (var carousel in carousels)
        {
            carousel.JCarouselEntitySettings = await _entitySettingService.LoadEntitySettingsAsync<JCarouselEntitySettings>(
                Plugin.EntityType,
                carousel.Id);
        }

        return carousels;
    }
}
