using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class DeliverySchedulingOrderModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryDate")]
        public string DeliveryTimeRange { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryTime")]
        public string DeliveryTime { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.ShippingMethodName")]
        public string ShippingMethodName { get; set; }
    }
}
