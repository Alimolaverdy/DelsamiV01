using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace ZarIran.Plugin.Misc.GoldPurchase.Components;

public class GoldWithdrawalLinkViewComponent : NopViewComponent
{
    public IViewComponentResult Invoke()
    {
        if (!string.Equals(RouteData.Values["controller"] as string, "Order", System.StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(RouteData.Values["action"] as string, "CustomerRewardPoints", System.StringComparison.OrdinalIgnoreCase))
            return Content(string.Empty);

        return View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/WithdrawalLink.cshtml");
    }
}
