using Nop.Core;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Domain
{
    public class PaymentMethodAmountRestriction : BaseEntity
    {
        public string PaymentMethodSystemName { get; set; }

        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }
    }
}
