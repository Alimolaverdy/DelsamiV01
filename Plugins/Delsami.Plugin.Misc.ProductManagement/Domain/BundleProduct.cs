using Nop.Core;

namespace Delsami.Plugin.Misc.ProductManagement.Domain
{
    public class BundleProduct : BaseEntity
    {
        public int ProductId1 { get; set; }
        public int ProductId2 { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
