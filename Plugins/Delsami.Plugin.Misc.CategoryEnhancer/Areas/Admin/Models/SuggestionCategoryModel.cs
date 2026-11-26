using Nop.Web.Framework.Models;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Areas.Admin.Models
{
    public record SuggestionCategoryModel : BaseNopModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SeName { get; set; } 
    }
}
