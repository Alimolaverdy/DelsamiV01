using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Models;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Factories
{
    public interface IDelsamiShippingModelFactory
    {
        Task<DelsamiShippingMethodStateProvinceRestrictionModel> PrepareDelsamiShippingMethodRestrictionModelAsync(DelsamiShippingMethodStateProvinceRestrictionModel model);
        Task<DelsamiShippingMethodStateProvinceRestrictionModel> PrepareDelsamiShippingMethodRestrictionModelAsync(int stateProvinceId);
        Task<ConfigurationModel> PrepareConfigureModelAsync(ConfigurationModel configurationModel);
    }
}
