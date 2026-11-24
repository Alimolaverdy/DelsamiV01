using Nop.Core;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Domain
{
    public class PaymentMethodCityRestriction : BaseEntity
    {
        public string PaymentMethodSystemName { get; set; }

        public int CityId { get; set; }
    } 
}
