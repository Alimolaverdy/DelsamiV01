using Delsami.Plugin.Shipping.ManageShippingMethod.Domain;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;

public class DelsamiShippingService : IDelsamiShippingService
{
    #region Fields
    private readonly IRepository<ShippingMethodCityRestriction> _shippingMethodCityRestrictionRepository;
    private readonly IRepository<ShippingMethod> _shippingMethodepository;

    #endregion

    #region Ctor
    public DelsamiShippingService(IRepository<ShippingMethodCityRestriction> shippingMethodStateProvinceRestriction, IRepository<ShippingMethod> shippingMethodepository)
    {
        _shippingMethodCityRestrictionRepository = shippingMethodStateProvinceRestriction;
        _shippingMethodepository = shippingMethodepository;
    }
    #endregion

    #region Methods

    public async Task<bool> CityRestrictionExistsAsync(ShippingMethod shippingMethod, int cityId)
    {
        if (shippingMethod == null)
            throw new ArgumentNullException(nameof(shippingMethod));

        var result = await _shippingMethodCityRestrictionRepository.Table.AnyAsync(smcm => smcm.ShippingMethodId == shippingMethod.Id && smcm.CityId == cityId);

        return result;
    }

    public async Task<IList<ShippingMethodCityRestriction>> GetShippingMethodCityMappingAsync (int shippingMethodId, int CityId)
    {
        var query = _shippingMethodCityRestrictionRepository.Table.Where(smcm =>
            smcm.ShippingMethodId == shippingMethodId && smcm.CityId == CityId);

        return await query.ToListAsync();
    }

    public async Task InsertShippingMethodCityMappingAsync(ShippingMethodCityRestriction shippingMethodCityRestriction)
    {
        await _shippingMethodCityRestrictionRepository.InsertAsync(shippingMethodCityRestriction);
    }

    public async Task DeleteShippingMethodCityMappingAsync(ShippingMethodCityRestriction shippingMethodCityRestriction)
    {
        await _shippingMethodCityRestrictionRepository.DeleteAsync(shippingMethodCityRestriction);
    }

    public async Task<ShippingMethod> GetShippingMethodByNameAsync(string name)
    {

        if (string.IsNullOrWhiteSpace(name))
            return null;

        name = name.Trim();

        var shippingMethods = await _shippingMethodepository.Table
            .Where(sm => sm.Name != null)
            .ToListAsync();

        var shippingMethod = shippingMethods
            .FirstOrDefault(sm => sm.Name.Trim().Equals(name, StringComparison.InvariantCultureIgnoreCase));

        return shippingMethod;
    }
    #endregion 
}
