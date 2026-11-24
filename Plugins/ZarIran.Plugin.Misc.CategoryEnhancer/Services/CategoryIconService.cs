using LinqToDB;
using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Services
{
    public class CategoryIconService : ICategoryIconService
    {
        
        #region Fields

        private readonly IRepository<CategoryIcon> _repo;

        #endregion

        #region Ctor

        public CategoryIconService(IRepository<CategoryIcon> reqpo)
        {
            _repo = reqpo;
        }

        #endregion

        #region Methods

        public async Task<CategoryIcon> GetByCategoryIdAsync(int categoryId)
        {
            return await _repo.Table.FirstOrDefaultAsync(i => i.CategoryId == categoryId);
        }

        public async Task<IList<CategoryIcon>> GetByCategoryIdsAsync(int[] categoryIds)
        {
            return await _repo.Table
                              .Where(x => categoryIds.Contains(x.CategoryId))
                              .ToListAsync();
        }


        public async Task InsertAsync(CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            var existingIcon = await _repo.Table.FirstOrDefaultAsync(x => x.CategoryId == categoryIcon.CategoryId);

            if (existingIcon != null)
            {
                existingIcon.IconId = categoryIcon.IconId;

                await _repo.UpdateAsync(existingIcon);
            }
            else
            {
                await _repo.InsertAsync(categoryIcon);
            }
        }

        #endregion

    }
}
