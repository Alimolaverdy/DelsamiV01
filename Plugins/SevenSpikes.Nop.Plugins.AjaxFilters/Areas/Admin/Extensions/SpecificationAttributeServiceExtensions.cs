using System.Collections.Generic;
using System.Linq;
using Nop.Core.Infrastructure;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Areas.Admin.Extensions;

public static class SpecificationAttributeServiceExtensions
{
    public static SpecificationAttribute GetSpecificationAttributeByName(
        this ISpecificationAttributeService specificationAttributeService,
        string name)
    {
        return EngineContext.Current
            .Resolve<IRepository<SpecificationAttribute>>()
            .Table
            .FirstOrDefault(attribute => attribute.Name == name);
    }

    public static IList<SpecificationAttribute> GetSpecificationAttributeByIds(
        this ISpecificationAttributeService specificationAttributeService,
        int[] ids)
    {
        if (ids == null || ids.Length == 0)
            return new List<SpecificationAttribute>();

        var attributesById = EngineContext.Current
            .Resolve<IRepository<SpecificationAttribute>>()
            .Table
            .Where(attribute => ids.Contains(attribute.Id))
            .ToDictionary(attribute => attribute.Id);

        return ids
            .Where(attributesById.ContainsKey)
            .Select(id => attributesById[id])
            .ToList();
    }
}
