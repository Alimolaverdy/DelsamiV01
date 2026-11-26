using Nop.Web.Models.Catalog;

namespace Delsami.Plugin.Misc.ProductManagement.Models
{
    public record DelsamiProductOverviewModel : ProductOverviewModel
    {
        //public bool IsBundle { get; set; }
        public int BundleItemsCount { get; set; }

        public bool HasTradeDiscount { get; set; }
    }
}
