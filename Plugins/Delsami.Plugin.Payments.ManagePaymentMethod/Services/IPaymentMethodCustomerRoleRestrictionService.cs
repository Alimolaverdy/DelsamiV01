using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IPaymentMethodCustomerRoleRestrictionService
    {
        Task<IList<PaymentMethodCustomerRoleRestriction>> GetAllCustomerRoleRestrictionsAsync();

        Task<IList<PaymentMethodCustomerRoleRestriction>> GetPaymentMethodCustomerRoleRestrictionsByIdsAsync(List<int> ids);

        Task InsertPaymentMethodCustomerRoleRestrictionAsync(List<PaymentMethodCustomerRoleRestriction> models);

        Task DeleteAllCustomerRoleRestrictionAsync();
    }
}
