using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services;

public class PaymentMethodAmountRestrictionService : IPaymentMethodAmountRestrictionService
{
    private readonly IRepository<PaymentMethodAmountRestriction> _repository;

    public PaymentMethodAmountRestrictionService(IRepository<PaymentMethodAmountRestriction> repository)
    {
        _repository = repository;
    }

    public async Task<List<PaymentMethodAmountRestriction>> GetAllAsync()
    {
        return await _repository.Table.ToListAsync();
    }

    public async Task InsertRangeAsync(List<PaymentMethodAmountRestriction> restrictions)
    {
        foreach (var item in restrictions)
        {
            if (item.Id == 0)
                await _repository.InsertAsync(item);
            else
                await _repository.UpdateAsync(item);
        }
    }


}
