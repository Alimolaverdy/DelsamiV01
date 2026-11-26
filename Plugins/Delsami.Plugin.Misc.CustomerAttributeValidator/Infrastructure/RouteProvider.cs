using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Infrastructure
{
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutBillingAddress",
                pattern: $"{lang}/checkout/billingaddress",
                defaults: new { controller = "CustomCheckout", action = "BillingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectBillingAddress",
                pattern: $"{lang}/checkout/selectbillingaddress",
                defaults: new { controller = "CustomCheckout", action = "SelectBillingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingAddress",
                pattern: $"{lang}/checkout/shippingaddress",
                defaults: new { controller = "CustomCheckout", action = "ShippingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectShippingAddress",
                pattern: $"{lang}/checkout/selectshippingaddress",
                defaults: new { controller = "CustomCheckout", action = "SelectShippingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutCustomerInfo",
                pattern: $"{lang}/checkout/customerinfo",
                defaults: new { controller = "CustomCheckout", action = "CustomerInfo" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutConfirm",
                pattern: $"{lang}/checkout/confirm",
                defaults: new { controller = "CustomCheckout", action = "Confirm" });

            //shopping cart
            endpointRouteBuilder.MapControllerRoute(name: "ShoppingCart",
                pattern: $"{lang}/cart/",
                defaults: new { controller = "CustomShoppingCart", action = "Cart" });
        }

    }
}
