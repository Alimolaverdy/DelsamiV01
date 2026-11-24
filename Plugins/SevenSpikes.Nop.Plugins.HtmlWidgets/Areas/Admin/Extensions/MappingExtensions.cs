using SevenSpikes.Nop.Framework.AutoMapper;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Models;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Areas.Admin.Extensions;

public static class MappingExtensions
{
    public static HtmlWidgetsSettingsModel ToModel(this HtmlWidgetsSettings htmlWidgetsSettings)
    {
        return AutoMapperConfiguration7Spikes.MapTo<HtmlWidgetsSettings, HtmlWidgetsSettingsModel>(htmlWidgetsSettings);
    }

    public static HtmlWidgetsSettings ToEntity(this HtmlWidgetsSettingsModel htmlWidgetsSettingsModel)
    {
        return AutoMapperConfiguration7Spikes.MapTo<HtmlWidgetsSettingsModel, HtmlWidgetsSettings>(htmlWidgetsSettingsModel);
    }

    public static HtmlWidgetModel ToModel(this HtmlWidget htmlWidget)
    {
        return AutoMapperConfiguration7Spikes.MapTo<HtmlWidget, HtmlWidgetModel>(htmlWidget);
    }

    public static HtmlWidget ToEntity(this HtmlWidgetModel htmlWidgetModel)
    {
        return AutoMapperConfiguration7Spikes.MapTo<HtmlWidgetModel, HtmlWidget>(htmlWidgetModel);
    }

    public static HtmlWidget ToEntity(this HtmlWidgetModel model, HtmlWidget destination)
    {
        return AutoMapperConfiguration7Spikes.MapTo<HtmlWidgetModel, HtmlWidget>(model, destination);
    }
}
