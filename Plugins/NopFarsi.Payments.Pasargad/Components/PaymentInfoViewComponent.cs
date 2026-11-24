using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace NopFarsiIR.Plugin.Payments.BankPasargad.Components
{
    [ViewComponent(Name = "BankPasargadPaymentInfo")]
    public class PaymentInfoViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return base.View("~/Plugins/NopFarsi.Payments.Pasargad/Views/PaymentInfo.cshtml");
        }
    }
}
