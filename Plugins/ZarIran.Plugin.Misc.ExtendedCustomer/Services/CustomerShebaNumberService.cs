using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using Nop.Core;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Services
{
    public class CustomerShebaNumberService : ICustomerShebaNumberService
    {
        #region Fields

        private readonly IRepository<CustomerShebaNumber> _customerShebaNumberRepo;

        #endregion

        #region Ctor

        public CustomerShebaNumberService(IRepository<CustomerShebaNumber> customerShebaNumberRepo)
        {
            _customerShebaNumberRepo = customerShebaNumberRepo;
        }

        #endregion

        #region Methods

        public virtual async Task InsertAsync(CustomerShebaNumber CustomerShebaNumberInfo)
        {
            await _customerShebaNumberRepo.InsertAsync(CustomerShebaNumberInfo);
        }

        public virtual async Task UpdateAsync(CustomerShebaNumber CustomerShebaNumberInfo)
        {
            await _customerShebaNumberRepo.UpdateAsync(CustomerShebaNumberInfo);
        }

        public virtual async Task DeleteAsync(CustomerShebaNumber CustomerShebaNumberInfo)
        {
            await _customerShebaNumberRepo.DeleteAsync(CustomerShebaNumberInfo);
        }

        public virtual async Task<IPagedList<CustomerShebaNumber>> GetCustomerShebaNumbersAsync(int? customerId = null,
                                                                                                int pageIndex = 0,
                                                                                                int pageSize = int.MaxValue)
        {
            var CustomerShebaNumbersInfo = await _customerShebaNumberRepo.GetAllPagedAsync(query =>
            {
                if (customerId.HasValue)
                    query = query.Where(w => w.CustomerId == customerId);

                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return CustomerShebaNumbersInfo;
        }

        public async Task<CustomerShebaNumber> GetCustomerShebaNumberAsync(int id)
        {
            return await _customerShebaNumberRepo.GetByIdAsync(id);
        }

        #endregion
    }
}
