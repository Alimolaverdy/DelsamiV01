using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services;

public interface IPaymentMethodRestrictionService
{
    Task<bool> CityRestrictionExistsAsync(string systemName, int CityId);

    Task<IList<PaymentMethodCityRestriction>> GetPaymentMethodCityMappingAsync(string paymentMethodSystemName, int CityId);

    Task InsertPaymentMethodRestrictionAsync(PaymentMethodCityRestriction shippingMethodCityRestriction);

    Task DeletePaymentMethodCityMappingAsync(PaymentMethodCityRestriction shippingMethodCityRestriction);
}
