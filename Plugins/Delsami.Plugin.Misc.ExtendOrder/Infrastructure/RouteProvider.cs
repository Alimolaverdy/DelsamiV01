using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Delsami.Plugin.Misc.ExtendedOrder.Infrastructure
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
                defaults: new { controller = "DelsamiOrder", action = "List", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(
                name: "OrderEdit",
                pattern: "Admin/Order/Edit/{id?}",
                defaults: new { controller = "DelsamiOrder", action = "Edit", area = "Admin" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "AddOrderItem",
                pattern: "Admin/Order/AddProductToOrderDetails",
                defaults: new { controller = "DelsamiOrder", action = "AddProductToOrderDetails", area = "Admin" }
            );

            endpointRouteBuilder.MapControllerRoute(
                name: "DelsamiOrderNoteDelete",
                pattern: "Admin/Order/OrderNoteDelete",
                defaults: new { controller = "DelsamiOrder", action = "OrderNoteDelete", area = "Admin" }
            );
        }
    }
}
