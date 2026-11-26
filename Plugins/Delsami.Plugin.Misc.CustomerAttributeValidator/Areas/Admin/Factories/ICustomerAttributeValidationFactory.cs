using Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Factories
{
    public interface ICustomerAttributeValidationFactory
    {
        Task<CustomerAttributeValidationSearchModel> PrepareCustomerAttributeValidationSearchModelAsync(CustomerAttributeValidationSearchModel searchModel);

        Task<CustomerAttributeValidationListModel> PrepareCustomerAttributeValidationListModelAsync(CustomerAttributeValidationSearchModel searchModel);

        Task<CustomerAttributeValidationModel> PrepareCustomerAttributeValidationModelAsync(CustomerAttributeValidationModel model, CustomerAttributeValidation customerAttributeValidation);
    }
}
