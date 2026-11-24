using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "CustomerShebaNumberList",
                pattern: $"customershebanumber/list",
                defaults: new { controller = "CustomerShebaNumber", action = "List" });

            endpointRouteBuilder.MapControllerRoute(name: "CustomerShebaNumberCreate",
                pattern: $"customershebanumber/create",
                defaults: new { controller = "CustomerShebaNumber", action = "Create" });

            endpointRouteBuilder.MapControllerRoute(name: "CustomerShebaNumberEdit",
                pattern: $"customershebanumber/edit/{{id:min(0)}}",
                defaults: new { controller = "CustomerShebaNumber", action = "Edit" });

            endpointRouteBuilder.MapControllerRoute(name: "CustomerShebaNumberDelete",
                pattern: $"customershebanumber/delete/{{id:min(0)}}",
                defaults: new { controller = "CustomerShebaNumber", action = "Delete" });

        }
    }
}
