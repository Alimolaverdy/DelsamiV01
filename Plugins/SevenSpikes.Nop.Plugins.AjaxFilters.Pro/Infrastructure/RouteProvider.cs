using SevenSpikes.Nop.Framework.Routing;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Pro.Infrastructure
{
  public class RouteProvider : BaseRouteProvider
  {
    protected override string PluginSystemName => "SevenSpikes.Nop.Plugins.AjaxFilters.Pro";

    public RouteProvider()
      : base()
    {
    }
  }
}
