using Nop.Core;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Domain
{
    public class CategoryIcon : BaseEntity
    {
        public int CategoryId { get; set; }
        public int IconId { get; set; }
    }
}