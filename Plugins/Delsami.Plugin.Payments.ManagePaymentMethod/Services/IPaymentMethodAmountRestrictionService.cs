using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services;

public interface IPaymentMethodAmountRestrictionService
{
    Task<List<PaymentMethodAmountRestriction>> GetAllAsync();

    Task InsertRangeAsync(List<PaymentMethodAmountRestriction> restrictions);

}
