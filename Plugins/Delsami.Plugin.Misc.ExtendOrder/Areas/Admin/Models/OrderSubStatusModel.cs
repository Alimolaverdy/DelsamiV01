using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record OrderSubStatusModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.Name")]

        public string Name { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.SystemName")]

        public string SystemName { get; set; }
    }
}
