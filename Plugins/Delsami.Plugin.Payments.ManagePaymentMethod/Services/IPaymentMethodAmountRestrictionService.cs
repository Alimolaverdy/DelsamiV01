using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services;

public interface IPaymentMethodAmountRestrictionService
{
    Task<List<PaymentMethodAmountRestriction>> GetAllAsync();

    Task InsertRangeAsync(List<PaymentMethodAmountRestriction> restrictions);

}
