using Delsami.Plugin.Misc.CustomerAttributeValidator.Domain;
using Nop.Core;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Services
{
    public interface ICustomerAttributeValidationService
    {
        Task<IPagedList<CustomerAttributeValidation>> GetCustomerAttributeValidationsAsync(
                                                                        int pageIndex = 0,
                                                                        int pageSize = int.MaxValue);

        Task InsertAsync(CustomerAttributeValidation customerAttributeValidation);

        Task UpdateAsync(CustomerAttributeValidation customerAttributeValidation);

        Task<CustomerAttributeValidation> GetByIdAsync(int id);

        Task<List<CustomerAttribute>> GetInvalidCustomerAttributesAsync(Customer customer);

        Task<List<CustomerAttribute>> GetInvalidCustomerAttributesAsync(string attributesXml);

        Task<List<CustomerAttribute>> GetNullOrEmptyRequiredCustomerAttributesAsync(string attributesXml);

        Task DeleteAsync(CustomerAttributeValidation customerAttributeValidation);

        Task<CustomerAttributeValidation> GetCustomerAttributeValidationByCustomerAttributeIdAsync(int customerAttributeId, int? editedEntityId = null);

        Task<bool> CurrentCustomerInfoCompletedAsync();

    }
}
