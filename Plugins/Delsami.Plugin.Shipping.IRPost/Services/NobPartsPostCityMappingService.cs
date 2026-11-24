using ZarIran.Plugin.Shipping.IRPost.Domain;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.IRPost.Services
{
    public class ZarIranPostCityMappingService : IZarIranPostCityMappingService
    {
        #region Fields

        private readonly IRepository<ZarIranPostCityMapping> _ZarIranPostCityMappingRepo;

        #endregion

        #region Ctor

        public ZarIranPostCityMappingService(IRepository<ZarIranPostCityMapping> ZarIranPostCityMappingRepo)
        {
            _ZarIranPostCityMappingRepo = ZarIranPostCityMappingRepo;
        }

        #endregion

        #region Methods

        public async Task<ZarIranPostCityMapping> GetZarIranTiapxCityMappingByZarIranCityIdAsync(int cityId)
        {
            return await _ZarIranPostCityMappingRepo.Table.FirstOrDefaultAsync(f => f.ZarIranCityId == cityId);
        }

        #endregion
    }
}
