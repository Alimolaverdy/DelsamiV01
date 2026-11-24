using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace NopFarsi.Payments.IranKish.Components
{
    public class IkcPaymentInfoViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/NopFarsi.Payments.IranKish/Views/PaymentInformation.cshtml");
        }
    }
}
