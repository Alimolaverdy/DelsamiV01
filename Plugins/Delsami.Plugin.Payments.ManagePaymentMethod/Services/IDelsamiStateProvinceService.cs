using Nop.Core.Domain.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Delsami.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IDelsamiStateProvinceService
    {
        Task<IList<StateProvince>> GetAllStateProvincesByCountryIdAsync(int countryId);
    }
}
