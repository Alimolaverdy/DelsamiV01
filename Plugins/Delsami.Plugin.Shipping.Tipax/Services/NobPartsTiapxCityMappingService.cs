using ZarIran.Plugin.Shipping.Tipax.Domain;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Tipax.Services
{
    public class ZarIranTiapxCityMappingService : IZarIranTipaxCityMappingService
    {
        #region Fields

        private readonly IRepository<ZarIranTipaxCityMapping> _ZarIranTiapxCityMappingRepo;

        #endregion

        #region Ctor

        public ZarIranTiapxCityMappingService(IRepository<ZarIranTipaxCityMapping> ZarIranTiapxCityMappingRepo)
        {
            _ZarIranTiapxCityMappingRepo = ZarIranTiapxCityMappingRepo;
        }

        #endregion

        #region Methods

        public async Task<ZarIranTipaxCityMapping> GetZarIranTiapxCityMappingByZarIranCityIdAsync(int cityId)
        {
            return await _ZarIranTiapxCityMappingRepo.Table.FirstOrDefaultAsync(f => f.ZarIranCityId == cityId);
        }

        public async Task InsertAsync(ZarIranTipaxCityMapping ZarIranTipaxCityMapping)
        {
            await _ZarIranTiapxCityMappingRepo.InsertAsync(ZarIranTipaxCityMapping);
        }
        #endregion
    }
}
