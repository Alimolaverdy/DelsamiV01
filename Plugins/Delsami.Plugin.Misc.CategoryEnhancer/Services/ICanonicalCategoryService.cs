using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Services
{
    public interface ICanonicalCategoryService
    {
        Task<CanonicalCategory> GetByCategoryIdAsync(int categoryId);

        Task DeleteByCategoryIdAsync(int categoryId);

        Task InsertAsync(CanonicalCategory canonicalCategory);

        Task<bool> IsValidCanonicalAsync(int canonicalCategoryId);
    } 
}
