using ZarIran.Plugin.Misc.ExtendedOrder.Enums;
using Nop.Core;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Domian
{
    public class OrderWarehouseLocation:BaseEntity
    {
        public int OrderId { get; set; }

        public WarehouseLocation Location { get; set; }
    }
}
