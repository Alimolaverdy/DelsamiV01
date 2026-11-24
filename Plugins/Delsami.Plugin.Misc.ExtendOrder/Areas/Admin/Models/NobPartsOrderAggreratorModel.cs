using Nop.Web.Areas.Admin.Models.Orders;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record ZarIranOrderAggreratorModel : OrderAggreratorModel
    {
        public string aggregatorsubtotalexcludetax { get; set; }

    }
}
