using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.OtherBrandProductModel
{
    public record OtherBrandProductModel : BaseNopEntityModel
    {
        public int ProductId2 { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.OtherBrandProduct.Fields.Product")]
        public string Product2Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.OtherBrandProduct.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
