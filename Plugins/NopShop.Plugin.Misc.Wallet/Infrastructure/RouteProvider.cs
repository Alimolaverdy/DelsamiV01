using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace NopShop.Plugin.Misc.Wallet.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : BaseRouteProvider , IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(WalletDefaults.ChargeWalletRoute, "Plugins/Wallet/Charge",
                new { controller = "Wallet", action = "Charge" });

            var lang = GetLanguageRoutePattern();

            //endpointRouteBuilder.MapControllerRoute(name: "CheckoutCompleted",
            //    pattern: $"{lang}/checkout/completed/{{orderId:int?}}",
            //    defaults: new { controller = "Wallet", action = "Completed" });

            //endpointRouteBuilder.MapControllerRoute(name: "OrderDetails",
            //   pattern: $"{lang}/orderdetails/{{orderId:min(0)}}",
            //   defaults: new { controller = "Wallet", action = "Details" });


            //endpointRouteBuilder.MapControllerRoute(name: "NopCheckoutCompleted",
            //    pattern: $"{lang}/nopcheckout/completed/{{orderId:int?}}",
            //    defaults: new { controller = "Checkout", action = "Completed" });

            //endpointRouteBuilder.MapControllerRoute(name: "NopOrderDetails",
            //   pattern: $"{lang}/noporderdetails/{{orderId:min(0)}}",
            //   defaults: new { controller = "Order", action = "Details" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => int.MaxValue;
    }
}