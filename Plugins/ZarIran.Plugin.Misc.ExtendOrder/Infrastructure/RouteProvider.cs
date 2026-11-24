using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "RefillBasket",
                pattern: $"order/checkout/",
                defaults: new { controller = "CustomExtendedOrder", action = "Checkout" });

            endpointRouteBuilder.MapControllerRoute(name: "Order",
                pattern: $"Admin/Order/List",
                defaults: new { controller = "ZarIranOrder", action = "List", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(
                name: "OrderEdit",
                pattern: "Admin/Order/Edit/{id?}",
                defaults: new { controller = "ZarIranOrder", action = "Edit", area = "Admin" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "AddOrderItem",
                pattern: "Admin/Order/AddProductToOrderDetails",
                defaults: new { controller = "ZarIranOrder", action = "AddProductToOrderDetails", area = "Admin" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "ZarIranOrderNoteDelete",
                pattern: "Admin/Order/OrderNoteDelete",
                defaults: new { controller = "ZarIranOrder", action = "OrderNoteDelete", area = "Admin" }
            );
        }
    }
}
