using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund
{
    public record DigiPayRefundSearchModel : BaseSearchModel
    {

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Refund.TrackingCode")]
        public string TrackingCodeKeyWord { get; set; }

        public DigiPayRefundModel RefundRecord { get; set; } = new();


    }
}
