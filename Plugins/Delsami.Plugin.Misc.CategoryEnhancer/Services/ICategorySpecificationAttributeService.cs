using Delsami.Plugin.Misc.CategoryEnhancer.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Services;

public interface ICategorySpecificationAttributeService
{
    Task InsertRangeAsync(List<CategorySpecificationAttribute> models);

    Task<List<CategorySpecificationAttribute>> GetByCategoryId(int categoryId);

    Task<List<CategorySpecificationAttribute>> GetByCategoryIds(List<int> categoryIds);

    Task DeleteAllByCategoryId(int categoryId);

}
