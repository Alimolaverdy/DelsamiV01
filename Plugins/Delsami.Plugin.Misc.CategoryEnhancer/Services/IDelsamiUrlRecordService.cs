using Nop.Core.Domain.Seo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Services
{
    public interface IDelsamiUrlRecordService
    {
        Task<List<UrlRecord>> GetSuggestionForCanonicalCategoriesAsync(string categoryName, string categorySeName, int currentEntityId);
    }
}
