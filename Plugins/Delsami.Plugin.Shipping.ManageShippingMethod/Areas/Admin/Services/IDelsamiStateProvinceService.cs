using Nop.Core.Domain.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;

public interface IDelsamiStateProvinceService
{
    Task<IList<StateProvince>> GetAllStateProvincesByCountryIdAsync(int countryId);
}
