using Nop.Core;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Domian
{
    public class OrderShippingMethod : BaseEntity
    {
        public int OrderId { get; set; }
        public int ShippingMethodId { get; set; }

    }
}
