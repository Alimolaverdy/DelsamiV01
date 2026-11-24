using Nop.Core.Domain.Seo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Services
{
    public interface IZarIranUrlRecordService
    {
        Task<List<UrlRecord>> GetSuggestionForCanonicalCategoriesAsync(string categoryName, string categorySeName, int currentEntityId);
    }
}
