using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using ZarIran.Plugin.Misc.ExtendedCustomer.Models.CustomerShebaNumbers;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Factories.CustomerShebaNumbers
{
    public interface ICustomerShebaNumberModelFactory
    {
        Task<CustomerShebaNumberListModel> PrepareCustomerShebaNumberModelListAsync(int? customerId = null);

        Task<CustomerShebaNumberSearchModel> PrepareCustomerShebaNumberSearchModelAsync(CustomerShebaNumberSearchModel searchModel);

        Task<CustomerShebaNumberListModel> PrepareShebaNumberListModelAsync(CustomerShebaNumberSearchModel searchModel, int? customerId = null);

        CustomerShebaNumberModel PrepareCustomerShebaNumberModel(CustomerShebaNumberModel customerShebaNumberModel, CustomerShebaNumber customerShebaNumber);
    }
}
