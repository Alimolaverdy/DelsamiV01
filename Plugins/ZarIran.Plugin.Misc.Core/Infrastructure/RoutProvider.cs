using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.Core.Infrastructure
{
    public class RoutProvider : IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
        }
    }
}
