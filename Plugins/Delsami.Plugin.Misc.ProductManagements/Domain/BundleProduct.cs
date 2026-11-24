using Nop.Core;

namespace ZarIran.Plugin.Misc.ProductManagements.Domain
{
    public class BundleProduct : BaseEntity
    {
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
