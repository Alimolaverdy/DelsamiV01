using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.BundleProducts;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.VirtualProducts;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.Products
{
    public record DelsamiProductModel : ProductModel
    {
        public DelsamiProductModel() : base()
        {
            VirtualProductSearchModel = new();
        }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.IsVirtualProduct")]
        public bool IsVirtualProduct { get; set; }

        public VirtualProductSearchModel VirtualProductSearchModel { get; set; }

        public int? MainProductId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MainProductName")]
        public string MainProductName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.VirtualProductsTextArea")]
        public string VirtualProductsTextArea { get; set; }

        public BundleProductSearchModel BundleProductSearchModel { get; set; }
    }
}
