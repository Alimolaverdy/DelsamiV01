using Nop.Core;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Domain
{
    public class PaymentMethodCustomerRoleRestriction : BaseEntity
    {
        public string PaymentMethodSystemName { get; set; }

        public int CustomerRoleId { get; set; }
    }
}
