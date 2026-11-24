using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.PaymentMethodDiscounts
{
    public record PaymentMethodDiscountModel : BaseNopEntityModel
    {
        public PaymentMethodDiscountModel()
        {
            AvailablePaymentMethodSystemNames = new();
        }

        public string PaymentMethodSystemName { get; set; }

        public int DiscountPercent { get; set; }

        public string StartDateStr { get; set; }

        public string EndDateStr { get; set; }

        public List<SelectListItem> AvailablePaymentMethodSystemNames { get; set; }

    }
}
