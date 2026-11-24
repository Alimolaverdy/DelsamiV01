using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Widgets.AddressLocation.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "ZarIranAddressLocationAddAddress",
                pattern: $"/customer/addressadd",
                defaults: new { controller = "AddressLocation", action = "AddressAdd" }
            );

            endpointRouteBuilder.MapControllerRoute(name: "ZarIranAddressLocationEditAddress",
                pattern: $"/customer/addressedit/{{addressId:min(0)}}",
                defaults: new { controller = "AddressLocation", action = "AddressEdit" }
            );
        }
    }
}
