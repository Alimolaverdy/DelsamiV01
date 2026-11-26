using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Domain
{
    public class PaymentMethodRewardPoint : BaseEntity
    {
        [NopResourceDisplayName("Delsami.Plugin.Shipping.ManageShippingMethod.PaymentMethodRewardPoint.OrderId")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.ManageShippingMethod.PaymentMethodRewardPoint.RewardPointHistoryId")]
        public int RewardPointHistoryId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Shipping.ManageShippingMethod.PaymentMethodRewardPoint.Status")]
        public string Status {  get; set; } 
    }
}
