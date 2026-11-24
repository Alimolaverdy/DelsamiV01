using Nop.Core.Domain.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;

public interface IZarIranStateProvinceService
{
    Task<IList<StateProvince>> GetAllStateProvincesByCountryIdAsync(int countryId);
}
