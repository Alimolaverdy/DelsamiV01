using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.OnePageCheckout.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "ZarIranOnePageCheckout",
                pattern: $"onepagecheckout",
                defaults: new { controller = "ZarIranCheckout", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute(name: "ZarIranOnePageCheckoutAddAddress",
                pattern: $"/customer/addressadd",
                defaults: new { controller = "OnePageCheckout", action = "AddressAdd" });

            endpointRouteBuilder.MapControllerRoute(
                name: " ZarIranOnePageCheckoutCheckUserVpn",
                pattern: "Plugins/OnePageCheckout/CheckUserVpn",
                defaults: new { controller = "ZarIranCheckout", action = "CheckUserVpn" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: " checkOutComplete",
                pattern: "checkout/completed",
                defaults: new { controller = "ZarIranCheckout", action = "Completed" }
            );
        }
    }
}
