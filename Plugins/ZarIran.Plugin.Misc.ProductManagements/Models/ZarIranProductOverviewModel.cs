using Nop.Web.Models.Catalog;

namespace ZarIran.Plugin.Misc.ProductManagements.Models
{
    public record ZarIranProductOverviewModel : ProductOverviewModel
    {
        //public bool IsBundle { get; set; }
        public int BundleItemsCount { get; set; }

        public bool HasTradeDiscount { get; set; }
    }
}
