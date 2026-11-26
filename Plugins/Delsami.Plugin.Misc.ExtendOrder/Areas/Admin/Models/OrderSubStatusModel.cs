using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record OrderSubStatusModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedOrder.SubStatus.Name")]

        public string Name { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedOrder.SubStatus.SystemName")]

        public string SystemName { get; set; }
    }
}
