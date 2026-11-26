using Nop.Core;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Domain
{
    public class PaymentMethodCityRestriction : BaseEntity
    {
        public string PaymentMethodSystemName { get; set; }

        public int CityId { get; set; }
    } 
}
