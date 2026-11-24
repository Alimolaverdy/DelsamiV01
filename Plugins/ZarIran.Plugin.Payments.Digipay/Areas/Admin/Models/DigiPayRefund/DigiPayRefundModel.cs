using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund
{
    public record DigiPayRefundModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.PaymentTrackingCode")]
		public string PaymentTrackingCode { get; set; }
        
        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.RefundTrackingCode")]
		public string RefundTrackingCode { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.Amount")]
        public string Amount { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.Status")]
        public bool Status { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.RefundDateTime")]
        public DateTime RefundDateTimeOnUtc { get; set; }
        public string RefundDateTimeOnUtcStr { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.Customer")]
        public string Customer { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.PaymentType")]
        public PaymentType PaymentType { get; set; }
        public string PaymentTypeStr { get; set; }

    }


}
