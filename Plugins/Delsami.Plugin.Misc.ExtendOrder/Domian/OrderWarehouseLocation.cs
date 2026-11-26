using Delsami.Plugin.Misc.ExtendedOrder.Enums;
using Nop.Core;

namespace Delsami.Plugin.Misc.ExtendedOrder.Domian
{
    public class OrderWarehouseLocation:BaseEntity
    {
        public int OrderId { get; set; }

        public WarehouseLocation Location { get; set; }
    }
}
