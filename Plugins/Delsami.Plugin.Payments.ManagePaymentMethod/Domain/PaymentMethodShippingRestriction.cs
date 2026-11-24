using Nop.Core;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Domain
{
    public class PaymentMethodShippingRestriction : BaseEntity
    {
        public string PaymentMethodSystemName { get; set; }

        public int ShippingMethodId { get; set; }
    }
}
