using Nop.Core.Domain.Orders;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public class ZarIranOrderAverageReportLine: OrderAverageReportLine
    {
        public decimal SumOrderSubtotalExcludeTaxs { get; set; }
    }
}
