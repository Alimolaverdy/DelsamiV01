using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services;

public interface IPaymentMethodShippingRestrictionService
{
    Task<bool> CityRestrictionExistsAsync(string systemName, int shippingMethodId);

    Task<IList<PaymentMethodShippingRestriction>> GetPaymentMethodShippingRestrictionMappingAsync(string paymentMethodSystemName, int shippingMethodId);

    Task<IList<PaymentMethodShippingRestriction>> GetAllAsync();

    Task InsertAsync(PaymentMethodShippingRestriction  paymentMethodShippingRestriction);

    Task DeleteAsync(PaymentMethodShippingRestriction  paymentMethodShippingRestriction);
}
