using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Directory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services
{
    public class ZarIranStateProvinceService : IZarIranStateProvinceService
    {
        #region Fields
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        #endregion

        #region Ctor
        public ZarIranStateProvinceService(IRepository<StateProvince> stateProvinceRepository, IStaticCacheManager staticCacheManager)
        {
            _stateProvinceRepository = stateProvinceRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods
        public async Task<IList<StateProvince>> GetAllStateProvincesByCountryIdAsync(int countryId)
        {
            var query = from sp in _stateProvinceRepository.Table.Where(i => i.CountryId == countryId)
                        orderby sp.DisplayOrder, sp.Name
                        where sp.Published
                        select sp;
            var stateProvinces = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesAllCacheKey), async () => await query.ToListAsync());

            return stateProvinces;
        }
        #endregion
    }
}
