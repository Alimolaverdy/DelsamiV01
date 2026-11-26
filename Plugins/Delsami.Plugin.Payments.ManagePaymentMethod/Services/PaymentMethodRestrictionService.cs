using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services
{
    public class PaymentMethodRestrictionService : IPaymentMethodRestrictionService
    {
        #region Fields

        private readonly IRepository<PaymentMethodCityRestriction> _PaymentMethodRestrictionRepo;

        #endregion

        #region Ctor

        public PaymentMethodRestrictionService(IRepository<PaymentMethodCityRestriction> PaymentMethodRestrictionRepo)
        {
            _PaymentMethodRestrictionRepo = PaymentMethodRestrictionRepo;
        }

        #endregion

        #region Methods

        public async Task<bool> CityRestrictionExistsAsync(string systemName, int stateProvinceId)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return false;

            var result = await _PaymentMethodRestrictionRepo.Table
                .AnyAsync(smcm => smcm.PaymentMethodSystemName == systemName && smcm.CityId == stateProvinceId);

            return result;
        }

        public async Task<IList<PaymentMethodCityRestriction>> GetPaymentMethodCityMappingAsync(string paymentMethodSystemName, int stateProvinceId)
        {
            var query = _PaymentMethodRestrictionRepo.Table.Where(smcm =>
                smcm.PaymentMethodSystemName == paymentMethodSystemName && smcm.CityId == stateProvinceId);

            return await query.ToListAsync();
        }

        public async Task InsertPaymentMethodRestrictionAsync(PaymentMethodCityRestriction shippingMethodStateProvinceRestriction)
        {
            await _PaymentMethodRestrictionRepo.InsertAsync(shippingMethodStateProvinceRestriction);
        }

        public async Task DeletePaymentMethodCityMappingAsync(PaymentMethodCityRestriction shippingMethodStateProvinceRestriction)
        {
            await _PaymentMethodRestrictionRepo.DeleteAsync(shippingMethodStateProvinceRestriction);
        }


        #endregion
    }
}
