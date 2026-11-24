using ZarIran.Plugin.Shipping.ManageShippingMethod.Domain;
using Nop.Core.Domain.Shipping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;

public interface IZarIranShippingService
{
    Task<bool> CityRestrictionExistsAsync(ShippingMethod shippingMethod, int CityId);
    Task<IList<ShippingMethodCityRestriction>> GetShippingMethodCityMappingAsync(int shippingMethodId, int CityId);
    Task InsertShippingMethodCityMappingAsync(ShippingMethodCityRestriction shippingMethodCityRestriction);
    Task DeleteShippingMethodCityMappingAsync(ShippingMethodCityRestriction shippingMethodCityRestriction);
    Task<ShippingMethod> GetShippingMethodByNameAsync(string name);
}
