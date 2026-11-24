using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Stores;
using SevenSpikes.Nop.Mappings.Domain;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure.Constants;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Services;

public class HtmlWidgetService : IHtmlWidgetService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IRepository<HtmlWidget> _htmlWidgetRepository;
    private readonly IRepository<EntityWidgetMapping> _entityWidgetMappingRepository;
    private readonly IStoreMappingService _storeMappingService;

    public HtmlWidgetService(
        IEventPublisher eventPublisher,
        IRepository<HtmlWidget> htmlWidgetRepository,
        IRepository<EntityWidgetMapping> entityWidgetMappingRepository,
        IStoreMappingService storeMappingService)
    {
        _eventPublisher = eventPublisher;
        _htmlWidgetRepository = htmlWidgetRepository;
        _entityWidgetMappingRepository = entityWidgetMappingRepository;
        _storeMappingService = storeMappingService;
    }

    public async Task<IList<HtmlWidget>> GetHtmlWidgetsByWidgetZoneAsync(string widgetZone)
    {
        var query = from htmlWidget in _htmlWidgetRepository.Table
                    join widgetMapping in _entityWidgetMappingRepository.Table
                        on htmlWidget.Id equals widgetMapping.EntityId
                    where (int)widgetMapping.EntityType == (int)Plugin.EntityType
                          && widgetMapping.WidgetZone == widgetZone
                          && htmlWidget.Visible
                    orderby widgetMapping.DisplayOrder
                    select htmlWidget;

        var matchingWidgets = await AsyncIQueryableExtensions.ToListAsync(query);
        var authorizedWidgets = new List<HtmlWidget>(matchingWidgets.Count);

        foreach (var htmlWidget in matchingWidgets)
        {
            if (await _storeMappingService.AuthorizeAsync(htmlWidget))
                authorizedWidgets.Add(htmlWidget);
        }

        return authorizedWidgets;
    }

    public async Task<IList<HtmlWidget>> GetAllHtmlWidgetsAsync()
    {
        return await AsyncIQueryableExtensions.ToListAsync(_htmlWidgetRepository.Table);
    }

    public Task<HtmlWidget> GetHtmlWidgetByIdAsync(int id)
    {
        return _htmlWidgetRepository.GetByIdAsync(
            id,
            (Func<IStaticCacheManager, CacheKey>)null,
            true);
    }

    public async Task InsertHtmlWidgetAsync(HtmlWidget htmlWidget)
    {
        if (htmlWidget == null)
            throw new ArgumentNullException(nameof(htmlWidget));

        await _htmlWidgetRepository.InsertAsync(htmlWidget, true);
        await _eventPublisher.EntityInsertedAsync(htmlWidget);
    }

    public async Task UpdateHtmlWidgetAsync(HtmlWidget htmlWidget)
    {
        if (htmlWidget == null)
            throw new ArgumentNullException(nameof(htmlWidget));

        await _htmlWidgetRepository.UpdateAsync(htmlWidget, true);
        await _eventPublisher.EntityUpdatedAsync(htmlWidget);
    }

    public async Task DeleteHtmlWidgetAsync(HtmlWidget htmlWidget)
    {
        if (htmlWidget == null)
            throw new ArgumentNullException(nameof(htmlWidget));

        await _htmlWidgetRepository.DeleteAsync(htmlWidget, true);
        await _eventPublisher.EntityDeletedAsync(htmlWidget);
    }
}
