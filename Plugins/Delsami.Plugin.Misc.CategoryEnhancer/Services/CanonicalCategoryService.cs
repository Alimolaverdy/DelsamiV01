using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Services
{
    public class CanonicalCategoryService : ICanonicalCategoryService
    {
        #region Fields

        private readonly IRepository<CanonicalCategory> _canonicalCategoryRepo;

        #endregion

        #region Ctor

        public CanonicalCategoryService(IRepository<CanonicalCategory> canonicalCategoryRepo)
        {
            _canonicalCategoryRepo = canonicalCategoryRepo;
        }

        #endregion

        #region Methods

        public async Task<CanonicalCategory> GetByCategoryIdAsync(int categoryId)
        {
          return  await _canonicalCategoryRepo.Table.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
        }

        public async Task DeleteByCategoryIdAsync(int categoryId)
        {
            await _canonicalCategoryRepo.DeleteAsync(x => x.CategoryId == categoryId);
        }

        public async Task InsertAsync(CanonicalCategory canonicalCategory)
        {
            if (canonicalCategory is null)
                throw new ArgumentNullException(nameof(canonicalCategory));

            await _canonicalCategoryRepo.InsertAsync(canonicalCategory);
        }

        public async Task<bool> IsValidCanonicalAsync(int canonicalCategoryId)
        {
            return !await _canonicalCategoryRepo.Table.AnyAsync(a => a.CategoryId == canonicalCategoryId);
        }

        #endregion
    }
}
