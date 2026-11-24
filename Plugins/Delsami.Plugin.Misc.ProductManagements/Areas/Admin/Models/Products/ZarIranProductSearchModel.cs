using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models;

public record ZarIranProductSearchModel : ProductSearchModel
{
    [NopResourceDisplayName("ZarIran.Misc.ProductSearch.ZarIranProductSearchModel.AdvancedSearch")]
    public bool AdvancedSearch { get; set; }
}
