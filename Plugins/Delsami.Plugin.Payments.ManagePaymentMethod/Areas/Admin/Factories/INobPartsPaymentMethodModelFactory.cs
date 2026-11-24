using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.ShippingMethodRestriction;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories
{
    public interface IZarIranPaymentMethodModelFactory
    {
        Task<ZarIranPaymentMethodRestrictionModel> PrepareZarIranPaymentMethodRestrictionModelAsync(ZarIranPaymentMethodRestrictionModel model);
        Task<ZarIranPaymentMethodRestrictionModel> PrepareZarIranPaymentMethodRestrictionModelAsync(int provinceId);
        Task<PaymentMethodShippingRestrictionModel> PreparePaymentMethodShippingRestrictionModelAsync();
        Task<ZarIranPaymentMethodCustomerRoleRestrictionModel> PrepareRoleRestrictionModelAsync();
        Task<ConfigurationModel> PrepareConfigureModelAsync(ConfigurationModel configurationModel);
    }
}
