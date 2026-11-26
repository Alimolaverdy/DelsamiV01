using Delsami.Plugin.Shipping.IRPost.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost.Services
{
    public interface IDelsamiPostCityMappingService
    {
        Task<DelsamiPostCityMapping> GetDelsamiTiapxCityMappingByDelsamiCityIdAsync(int cityId);
    }
}
