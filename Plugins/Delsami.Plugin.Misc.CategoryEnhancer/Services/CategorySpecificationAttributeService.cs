using System.Linq;
using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using LinqToDB;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Services;

public class CategorySpecificationAttributeService : ICategorySpecificationAttributeService
{
    private readonly IRepository<CategorySpecificationAttribute> _repository;

    public CategorySpecificationAttributeService(IRepository<CategorySpecificationAttribute> repository)
    {
        _repository = repository;
    }

    public async Task DeleteAllByCategoryId(int categoryId)
    {
        var specList = await GetByCategoryId(categoryId);

        if (specList != null && specList.Any())
            await _repository.DeleteAsync(specList);

    }

    public async Task<List<CategorySpecificationAttribute>> GetByCategoryId(int categoryId)
    {
        return await _repository.Table.Where(a => a.CategoryId == categoryId).ToListAsync();
    }

    public async Task<List<CategorySpecificationAttribute>> GetByCategoryIds(List<int> categoryIds)
    {
        var list= await _repository.Table.Where(a => categoryIds.Contains(a.CategoryId)).ToListAsync();
        return list.DistinctBy(a=>a.SpecificationAttributeId).ToList();
    }

    public async Task InsertRangeAsync(List<CategorySpecificationAttribute> models)
    {
        await _repository.InsertAsync(models);
    }
}
