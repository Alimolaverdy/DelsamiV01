using Nop.Web.Areas.Admin.Models.Orders;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record DelsamiOrderAggreratorModel : OrderAggreratorModel
    {
        public string aggregatorsubtotalexcludetax { get; set; }

    }
}
