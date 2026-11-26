using Nop.Core;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Domain
{
    public class CanonicalCategory : BaseEntity
    {
        public int CategoryId { get; set; }

        public int CanonicalCategoryId { get; set; }
    }
}
