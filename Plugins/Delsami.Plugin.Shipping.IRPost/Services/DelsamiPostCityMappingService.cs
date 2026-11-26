using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost.Services
{
    public class DelsamiPostCityMappingService : IDelsamiPostCityMappingService
    {
        #region Fields

        private readonly IRepository<DelsamiPostCityMapping> _DelsamiPostCityMappingRepo;

        #endregion

        #region Ctor

        public DelsamiPostCityMappingService(IRepository<DelsamiPostCityMapping> DelsamiPostCityMappingRepo)
        {
            _DelsamiPostCityMappingRepo = DelsamiPostCityMappingRepo;
        }

        #endregion

        #region Methods

        public async Task<DelsamiPostCityMapping> GetDelsamiTiapxCityMappingByDelsamiCityIdAsync(int cityId)
        {
            return await _DelsamiPostCityMappingRepo.Table.FirstOrDefaultAsync(f => f.DelsamiCityId == cityId);
        }

        #endregion
    }
}
