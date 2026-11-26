using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Areas.Admin.Models
{
    public record DelsamiCategoryModel : CategoryModel
    {
        public DelsamiCategoryModel()
        {
            SuggestionCategories = new();
        }

        [UIHint("Picture")]
        [NopResourceDisplayName("Delsami.Plugin.Misc.CategoryEnhancer.Icon")]
        public int IconId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CategoryEnhancer.Category.Fields.Name")]
        public int CanonicalCategoryId { get; set; }

        public List<SuggestionCategoryModel> SuggestionCategories { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.CategoryEnhancer.Category.Fields.SelectedSpecAttributes")]
        public IList<int> SelectedSpecAttributesIds { get; set; }=new List<int>();

        public IList<SelectListItem> AvailableSpecAttributes { get; set; } = new List<SelectListItem>();
    }
}
