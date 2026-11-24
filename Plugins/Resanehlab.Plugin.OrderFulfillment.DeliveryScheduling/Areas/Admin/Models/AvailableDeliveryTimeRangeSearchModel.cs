using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record AvailableDeliveryTimeRangeSearchModel : BaseSearchModel
    {
        public int StoreId { get; set; }
        public int? ExceptionDateId { get; set; }

        public bool IsCustomTimeRange { get; set; }


    }
}
