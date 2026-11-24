using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IPaymentMethodCustomerRoleRestrictionService
    {
        Task<IList<PaymentMethodCustomerRoleRestriction>> GetAllCustomerRoleRestrictionsAsync();

        Task<IList<PaymentMethodCustomerRoleRestriction>> GetPaymentMethodCustomerRoleRestrictionsByIdsAsync(List<int> ids);

        Task InsertPaymentMethodCustomerRoleRestrictionAsync(List<PaymentMethodCustomerRoleRestriction> models);

        Task DeleteAllCustomerRoleRestrictionAsync();
    }
}
