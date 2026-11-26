using Nop.Core;

namespace Delsami.Plugin.Misc.ExtendedOrder.Domian
{
    public class OrderShippingMethod : BaseEntity
    {
        public int OrderId { get; set; }
        public int ShippingMethodId { get; set; }

    }
}
