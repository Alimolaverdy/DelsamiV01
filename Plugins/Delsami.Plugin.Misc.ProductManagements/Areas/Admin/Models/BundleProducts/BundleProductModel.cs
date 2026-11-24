using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts
{
    public record BundleProductModel : BaseNopEntityModel
    {
        public int ProductId2 { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.BundleProduct.Fields.Product")]
        public string Product2Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.BundleProduct.Fields.Count")]
        public int Count { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.BundleProduct.Fields.Price")]
        public decimal Price { get; set; }

    }
}
