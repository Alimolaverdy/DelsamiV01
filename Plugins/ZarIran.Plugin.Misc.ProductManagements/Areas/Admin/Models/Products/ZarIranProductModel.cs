using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.OtherBrandProductModel;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.BundleProducts;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.VirtualProducts;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.Products
{
    public record ZarIranProductModel : ProductModel
    {
        public ZarIranProductModel() : base()
        {
            VirtualProductSearchModel = new();
            OtherBrandProductSearchModel = new();
        }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.IsVirtualProduct")]
        public bool IsVirtualProduct { get; set; }

        public VirtualProductSearchModel VirtualProductSearchModel { get; set; }

        public int? MainProductId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.MainProductName")]
        public string MainProductName { get; set; }

        public OtherBrandProductSearchModel OtherBrandProductSearchModel { get; set; }
        public BundleProductSearchModel BundleProductSearchModel { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.VirtualProductsTextArea")]
        public string VirtualProductsTextArea { get; set; }


        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.MakingFee")]
        public decimal MakingFee { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.OldMakingFee")]
        public decimal OldMakingFee { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.OldProfitPercent")]
        public decimal OldProfitPercent { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.ProfitPercent")]
        public decimal ProfitPercent { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.OldCoWorkerProfitPercent")]
        public decimal OldCoWorkerProfitPercent { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.CoWorkerProfitPercent")]
        public decimal CoWorkerProfitPercent { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.CoWorkerMakingFee")]
        public decimal CoWorkerMakingFee { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.OldCoWorkerMakingFee")]
        public decimal OldCoWorkerMakingFee { get; set; }

        [NopResourceDisplayName("ZarIran.Misc.ProductManagements.Fields.AdditionalAmount")]
        public decimal AdditionalAmount { get; set; }

    }
}
