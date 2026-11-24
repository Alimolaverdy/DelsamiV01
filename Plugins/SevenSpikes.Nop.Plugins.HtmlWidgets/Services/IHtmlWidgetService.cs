using System.Collections.Generic;
using System.Threading.Tasks;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Services;

public interface IHtmlWidgetService
{
    Task<IList<HtmlWidget>> GetHtmlWidgetsByWidgetZoneAsync(string widgetZone);

    Task<IList<HtmlWidget>> GetAllHtmlWidgetsAsync();

    Task<HtmlWidget> GetHtmlWidgetByIdAsync(int id);

    Task InsertHtmlWidgetAsync(HtmlWidget htmlWidget);

    Task UpdateHtmlWidgetAsync(HtmlWidget htmlWidget);

    Task DeleteHtmlWidgetAsync(HtmlWidget htmlWidget);
}
