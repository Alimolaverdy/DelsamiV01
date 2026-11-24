using System.Collections.Generic;
using System.Linq;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Areas.Admin.Extensions;

public static class ProductAttributeServiceExtensions
{
    public static ProductAttribute GetProductAttributeByName(
        this IProductAttributeService productAttributeService,
        string name)
    {
        return EngineContext.Current
            .Resolve<IRepository<ProductAttribute>>()
            .Table
            .FirstOrDefault(attribute => attribute.Name == name);
    }

    public static IList<ProductAttribute> GetProductAttributesByIds(
        this IProductAttributeService productAttributeService,
        int[] ids)
    {
        if (ids == null || ids.Length == 0)
            return new List<ProductAttribute>();

        var attributesById = EngineContext.Current
            .Resolve<IRepository<ProductAttribute>>()
            .Table
            .Where(attribute => ids.Contains(attribute.Id))
            .ToDictionary(attribute => attribute.Id);

        return ids
            .Where(attributesById.ContainsKey)
            .Select(id => attributesById[id])
            .ToList();
    }
}
