using Nop.Core;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Domain;

public class CategorySpecificationAttribute:BaseEntity
{
    public int CategoryId { get; set; }

    public int SpecificationAttributeId { get; set; }
}
