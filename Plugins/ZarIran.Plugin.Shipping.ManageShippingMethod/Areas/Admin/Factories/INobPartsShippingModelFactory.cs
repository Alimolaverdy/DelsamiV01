using ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Models;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Factories
{
    public interface IZarIranShippingModelFactory
    {
        Task<ZarIranShippingMethodStateProvinceRestrictionModel> PrepareZarIranShippingMethodRestrictionModelAsync(ZarIranShippingMethodStateProvinceRestrictionModel model);
        Task<ZarIranShippingMethodStateProvinceRestrictionModel> PrepareZarIranShippingMethodRestrictionModelAsync(int stateProvinceId);
        Task<ConfigurationModel> PrepareConfigureModelAsync(ConfigurationModel configurationModel);
    }
}
