using Delsami.Plugin.Misc.CategoryEnhancer.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Services
{
    public interface ICategoryIconService
    {
        Task InsertAsync(CategoryIcon categoryIcon);
        Task<CategoryIcon> GetByCategoryIdAsync(int categoryId);
        Task<IList<CategoryIcon>> GetByCategoryIdsAsync(int[] categoryIds);
    }
}
