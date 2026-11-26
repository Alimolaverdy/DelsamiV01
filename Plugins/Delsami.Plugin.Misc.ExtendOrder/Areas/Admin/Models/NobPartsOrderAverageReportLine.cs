using Nop.Core.Domain.Orders;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public class DelsamiOrderAverageReportLine: OrderAverageReportLine
    {
        public decimal SumOrderSubtotalExcludeTaxs { get; set; }
    }
}
