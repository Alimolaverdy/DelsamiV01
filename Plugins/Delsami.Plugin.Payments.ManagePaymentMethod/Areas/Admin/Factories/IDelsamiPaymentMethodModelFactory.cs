using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.ShippingMethodRestriction;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories
{
    public interface IDelsamiPaymentMethodModelFactory
    {
        Task<DelsamiPaymentMethodRestrictionModel> PrepareDelsamiPaymentMethodRestrictionModelAsync(DelsamiPaymentMethodRestrictionModel model);
        Task<DelsamiPaymentMethodRestrictionModel> PrepareDelsamiPaymentMethodRestrictionModelAsync(int provinceId);
        Task<PaymentMethodShippingRestrictionModel> PreparePaymentMethodShippingRestrictionModelAsync();
        Task<DelsamiPaymentMethodCustomerRoleRestrictionModel> PrepareRoleRestrictionModelAsync();
        Task<ConfigurationModel> PrepareConfigureModelAsync(ConfigurationModel configurationModel);
    }
}
