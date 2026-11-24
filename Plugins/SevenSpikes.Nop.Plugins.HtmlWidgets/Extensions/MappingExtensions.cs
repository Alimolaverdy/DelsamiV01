using SevenSpikes.Nop.Framework.AutoMapper;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Domain;
using SevenSpikes.Nop.Plugins.HtmlWidgets.Models;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Extensions;

public static class MappingExtensions
{
    public static HtmlWidgetsPublicModel ToPublicModel(this HtmlWidget htmlWidget)
    {
        return AutoMapperConfiguration7Spikes.MapTo<HtmlWidget, HtmlWidgetsPublicModel>(htmlWidget);
    }
}
