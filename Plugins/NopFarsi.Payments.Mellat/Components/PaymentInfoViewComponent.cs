using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace NopFarsiIR.Payments.Mellat.Components
{
    public class PaymentInfoViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/NopFarsiIR.Payments.Mellat/Views/PaymentMellat/PaymentInfo.cshtml");
        }
    }
}
