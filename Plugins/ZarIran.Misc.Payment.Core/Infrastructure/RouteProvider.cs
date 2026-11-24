using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Payment.Core.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //PaymentError
            endpointRouteBuilder.MapControllerRoute(name: "Checkout/PaymentError",
                pattern: $"Checkout/PaymentError",
                defaults: new { controller = "ZarIranErrorPage", action = "PaymentError" });

        }
    }
}
