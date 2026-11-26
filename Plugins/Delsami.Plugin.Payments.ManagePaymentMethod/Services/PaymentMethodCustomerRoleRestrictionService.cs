using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services
{
    public class PaymentMethodCustomerRoleRestrictionService : IPaymentMethodCustomerRoleRestrictionService
    {
        #region Fields

        private readonly IRepository<PaymentMethodCustomerRoleRestriction> _PaymentMethodCustomerRoleRestrictionRepo;

        #endregion

        #region Ctor

        public PaymentMethodCustomerRoleRestrictionService(IRepository<PaymentMethodCustomerRoleRestriction> PaymentMethodCustomerRoleRestrictionRepo)
        {
            _PaymentMethodCustomerRoleRestrictionRepo = PaymentMethodCustomerRoleRestrictionRepo;
        }

        #endregion

        #region Methods

        public async Task<IList<PaymentMethodCustomerRoleRestriction>> GetAllCustomerRoleRestrictionsAsync()
        {
            return await _PaymentMethodCustomerRoleRestrictionRepo.Table.ToListAsync();

        }

        public async Task DeleteAllCustomerRoleRestrictionAsync()
        {
            await _PaymentMethodCustomerRoleRestrictionRepo.TruncateAsync();
        }

        public async Task InsertPaymentMethodCustomerRoleRestrictionAsync(List<PaymentMethodCustomerRoleRestriction> models)
        {
            await _PaymentMethodCustomerRoleRestrictionRepo.InsertAsync(models); 
        }

        public async Task<IList<PaymentMethodCustomerRoleRestriction>> GetPaymentMethodCustomerRoleRestrictionsByIdsAsync(List<int> ids)
        {
            return await _PaymentMethodCustomerRoleRestrictionRepo.Table.Where(a=>ids.Contains(a.CustomerRoleId)).ToListAsync();
        }

        #endregion
    }
}
