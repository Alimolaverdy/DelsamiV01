using Nop.Core;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Domain
{
    public class CanonicalCategory : BaseEntity
    {
        public int CategoryId { get; set; }

        public int CanonicalCategoryId { get; set; }
    }
}
