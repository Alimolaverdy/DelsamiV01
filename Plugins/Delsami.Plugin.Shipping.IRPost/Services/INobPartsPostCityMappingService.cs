using ZarIran.Plugin.Shipping.IRPost.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.IRPost.Services
{
    public interface IZarIranPostCityMappingService
    {
        Task<ZarIranPostCityMapping> GetZarIranTiapxCityMappingByZarIranCityIdAsync(int cityId);
    }
}
