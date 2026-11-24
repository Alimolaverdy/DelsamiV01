using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace ZarIran.Plugin.Misc.GoldPurchase.Infrastructure;

public class RouteProvider : BaseRouteProvider, IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(
            GoldPurchaseDefaults.ChargeRoute,
            "gold-purchase",
            new { controller = "GoldPurchase", action = "Buy" });

        endpointRouteBuilder.MapControllerRoute(
            GoldPurchaseDefaults.WithdrawalRoute,
            "gold-withdrawal",
            new { controller = "GoldWithdrawal", action = "Index" });

        endpointRouteBuilder.MapControllerRoute(
            GoldPurchaseDefaults.SellRoute,
            "gold-sell",
            new { controller = "GoldPurchase", action = "Sell" });

    }

    public int Priority => int.MaxValue;
}
