using Nop.Core;
using System;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Domain
{
    public class PaymentMethodDiscount : BaseEntity
    {
        public string PaymentMethodSystemName { get; set; }

        public int DiscountPercent { get; set; }

        public DateTime StartDateTimeOnUtc { get; set; }

        public DateTime? EndDateTimeOnUtc { get; set; }
    }
}
