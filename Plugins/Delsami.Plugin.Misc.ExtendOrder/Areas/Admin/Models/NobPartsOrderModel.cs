using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record ZarIranOrderModel : OrderModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedOrder.BriefAddress")]
        public string BriefAddress { get; set; }
        public string ShippingPersonFullName { get; set; }
        public string CustomerRoles { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedOrder.SubStatusId")]
        public int SubStatusId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.ExtendedOrder.SubStatus")]
        public string SubStatus { get; set; }

        public int ShippingMethodId { get; set; }

        
    }
}
