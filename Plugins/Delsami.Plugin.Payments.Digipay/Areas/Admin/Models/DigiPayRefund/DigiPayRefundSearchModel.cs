using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund
{
    public record DigiPayRefundSearchModel : BaseSearchModel
    {

        [NopResourceDisplayName("Delsami.Plugin.Payments.DigiPay.Refund.TrackingCode")]
        public string TrackingCodeKeyWord { get; set; }

        public DigiPayRefundModel RefundRecord { get; set; } = new();


    }
}
