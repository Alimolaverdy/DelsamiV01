using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.CarInfo.Infrastrcuture
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //endpointRouteBuilder.MapControllerRoute(name: "DecreaseQuantity",
            //    pattern: $"shoppingCart/decreasequantity",
            //    defaults: new { controller = "ShoppingCart", action = "DecreaseQuantity" });

            //endpointRouteBuilder.MapControllerRoute(name: "IncreaseQuantity",
            //    pattern: $"shoppingCart/increasequantity",
            //    defaults: new { controller = "ShoppingCart", action = "IncreaseQuantity" });

            endpointRouteBuilder.MapControllerRoute(name: "UpdateQuantity",
                pattern: $"shoppingCart/updatequantity",
                defaults: new { controller = "ShoppingCart", action = "UpdateQuantity" });

            endpointRouteBuilder.MapControllerRoute(name: "OrderTotals",
                pattern: $"shoppingCart/orderTotals",
                defaults: new { controller = "ShoppingCart", action = "GetOrderTotals" });


            endpointRouteBuilder.MapControllerRoute(name: "ZarIranCart",
                pattern: $"cart",
                defaults: new { controller = "ShoppingCart", action = "CustomCart" });

            //endpointRouteBuilder.MapControllerRoute(name: "CustomerCarAdd",
            //    pattern: $"customer/caradd",
            //    defaults: new { controller = "CustomerCar", action = "Create" });

            //endpointRouteBuilder.MapControllerRoute(name: "CustomerCarEdit",
            //    pattern: $"customer/caredit/{{id:min(0)}}",
            //    defaults: new { controller = "CustomerCar", action = "Edit" });


            //endpointRouteBuilder.MapControllerRoute(name: "CarGetModels",
            //    pattern: $"customer/car/getmodels",
            //    defaults: new { controller = "CustomerCar", action = "GetModels" });

            //endpointRouteBuilder.MapControllerRoute(name: "CarGetYears",
            //  pattern: $"customer/car/getyears",
            //  defaults: new { controller = "CustomerCar", action = "GetYears" });

            ////CarServiceReminder
            //endpointRouteBuilder.MapControllerRoute(name: "CarServiceReminderList",
            //    pattern: $"customer/carservicereminders",
            //    defaults: new { controller = "CarServiceReminder", action = "List" });

            //endpointRouteBuilder.MapControllerRoute(name: "CarServiceReminderAdd",
            //    pattern: $"customer/carservicereminderadd",
            //    defaults: new { controller = "CarServiceReminder", action = "Create" });

            //endpointRouteBuilder.MapControllerRoute(name: "CarServiceReminderEdit",
            //    pattern: $"customer/carservicereminderedit/{{id:min(0)}}",
            //    defaults: new { controller = "CarServiceReminder", action = "Edit" });
        }
    }
}
