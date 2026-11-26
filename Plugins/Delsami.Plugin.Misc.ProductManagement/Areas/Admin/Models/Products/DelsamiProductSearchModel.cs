using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models;

public record DelsamiProductSearchModel : ProductSearchModel
{
    [NopResourceDisplayName("Delsami.Misc.ProductSearch.DelsamiProductSearchModel.AdvancedSearch")]
    public bool AdvancedSearch { get; set; }
}
