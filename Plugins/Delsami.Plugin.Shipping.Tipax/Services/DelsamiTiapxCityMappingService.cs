using Delsami.Plugin.Shipping.Tipax.Domain;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Tipax.Services
{
    public class DelsamiTiapxCityMappingService : IDelsamiTipaxCityMappingService
    {
        #region Fields

        private readonly IRepository<DelsamiTipaxCityMapping> _DelsamiTiapxCityMappingRepo;

        #endregion

        #region Ctor

        public DelsamiTiapxCityMappingService(IRepository<DelsamiTipaxCityMapping> DelsamiTiapxCityMappingRepo)
        {
            _DelsamiTiapxCityMappingRepo = DelsamiTiapxCityMappingRepo;
        }

        #endregion

        #region Methods

        public async Task<DelsamiTipaxCityMapping> GetDelsamiTiapxCityMappingByDelsamiCityIdAsync(int cityId)
        {
            return await _DelsamiTiapxCityMappingRepo.Table.FirstOrDefaultAsync(f => f.DelsamiCityId == cityId);
        }

        public async Task InsertAsync(DelsamiTipaxCityMapping DelsamiTipaxCityMapping)
        {
            await _DelsamiTiapxCityMappingRepo.InsertAsync(DelsamiTipaxCityMapping);
        }
        #endregion
    }
}
