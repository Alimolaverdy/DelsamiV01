using Delsami.Plugin.Shipping.Tipax.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Tipax.Services
{
    public interface IDelsamiTipaxCityMappingService
    {
        Task<DelsamiTipaxCityMapping> GetDelsamiTiapxCityMappingByDelsamiCityIdAsync(int cityId);

        Task InsertAsync(DelsamiTipaxCityMapping DelsamiTipaxCityMapping);
    }
}
