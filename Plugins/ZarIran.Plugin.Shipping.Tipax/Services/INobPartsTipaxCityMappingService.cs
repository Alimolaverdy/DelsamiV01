using ZarIran.Plugin.Shipping.Tipax.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Tipax.Services
{
    public interface IZarIranTipaxCityMappingService
    {
        Task<ZarIranTipaxCityMapping> GetZarIranTiapxCityMappingByZarIranCityIdAsync(int cityId);

        Task InsertAsync(ZarIranTipaxCityMapping ZarIranTipaxCityMapping);
    }
}
