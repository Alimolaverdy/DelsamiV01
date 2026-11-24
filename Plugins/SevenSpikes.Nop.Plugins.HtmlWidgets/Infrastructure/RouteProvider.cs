using SevenSpikes.Nop.Framework.Routing;

namespace SevenSpikes.Nop.Plugins.HtmlWidgets.Infrastructure;

public class RouteProvider : BaseRouteProvider
{
    protected override string PluginSystemName => "SevenSpikes.Nop.Plugins.HtmlWidgets";

    public RouteProvider()
        : base(true)
    {
    }
}
