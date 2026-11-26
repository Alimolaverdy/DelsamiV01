using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund
{
    public enum PaymentType
    {
        IPG = 0,
        BNPL = 13,
        Wallet = 11,
        Credit = 5,
        CreditCard = 24,
    }
}
