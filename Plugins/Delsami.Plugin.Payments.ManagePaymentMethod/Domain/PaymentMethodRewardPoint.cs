using ZarIran.Plugin.Payments.ManagePaymentMethod.Enums;
using Nop.Core;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Domain
{
    public class PaymentMethodRewardPoint : BaseEntity
    {
        public int OrderId { get; set; }

        public int RewardPointHistoryId { get; set; }

        public RewardPointStatus Status { get; set; }
    }
}
