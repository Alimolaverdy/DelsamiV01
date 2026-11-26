using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services;

public class PaymentMethodShippingRestrictionService : IPaymentMethodShippingRestrictionService
{
    #region Fields

    private readonly IRepository<PaymentMethodShippingRestriction> _PaymentMethodRestrictionRepo;

    #endregion

    #region Ctor

    public PaymentMethodShippingRestrictionService(IRepository<PaymentMethodShippingRestriction> PaymentMethodRestrictionRepo)
    {
        _PaymentMethodRestrictionRepo = PaymentMethodRestrictionRepo;
    }

    #endregion

    #region Methods

    public async Task<IList<PaymentMethodShippingRestriction>> GetPaymentMethodShippingRestrictionMappingAsync(string paymentMethodSystemName, int shippingMethodId)
    {
        return await _PaymentMethodRestrictionRepo.Table.Where(
               smcm => smcm.PaymentMethodSystemName == paymentMethodSystemName
               && smcm.ShippingMethodId == shippingMethodId).ToListAsync();
    }

    public async Task InsertAsync(PaymentMethodShippingRestriction paymentMethodShippingRestriction)
    {
        await _PaymentMethodRestrictionRepo.InsertAsync(paymentMethodShippingRestriction);
    }

    public async Task DeleteAsync(PaymentMethodShippingRestriction paymentMethodShippingRestriction)
    {
        await _PaymentMethodRestrictionRepo.DeleteAsync(paymentMethodShippingRestriction);
    }

    public async Task<bool> CityRestrictionExistsAsync(string systemName, int shippingMethodId)
    {
            if (string.IsNullOrWhiteSpace(systemName))
                return false;

            var result = await _PaymentMethodRestrictionRepo.Table
                .AnyAsync(smcm => smcm.PaymentMethodSystemName == systemName && smcm.ShippingMethodId == shippingMethodId);

            return result;
        
    }

    public async Task<IList<PaymentMethodShippingRestriction>> GetAllAsync()
    {
      return await _PaymentMethodRestrictionRepo.Table.ToListAsync();
    }


    #endregion
}