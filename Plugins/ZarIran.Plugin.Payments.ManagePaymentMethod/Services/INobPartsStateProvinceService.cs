using Nop.Core.Domain.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services
{
    public interface IZarIranStateProvinceService
    {
        Task<IList<StateProvince>> GetAllStateProvincesByCountryIdAsync(int countryId);
    }
}
