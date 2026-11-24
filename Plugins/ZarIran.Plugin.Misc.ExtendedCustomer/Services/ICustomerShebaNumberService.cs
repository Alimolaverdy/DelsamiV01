using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using Nop.Core;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Services
{
    public interface ICustomerShebaNumberService
    {
        Task InsertAsync(CustomerShebaNumber CustomerShebaNumberInfo);

        Task UpdateAsync(CustomerShebaNumber CustomerShebaNumberInfo);

        Task DeleteAsync(CustomerShebaNumber CustomerShebaNumberInfo);

        Task<IPagedList<CustomerShebaNumber>> GetCustomerShebaNumbersAsync(int? customerId = null,
                                                                           int pageIndex = 0,
                                                                           int pageSize = int.MaxValue);
        Task<CustomerShebaNumber> GetCustomerShebaNumberAsync(int id);

    }
}
