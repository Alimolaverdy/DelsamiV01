using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public record ExceptionDateSearchModel : BaseSearchModel
    {
        public int StoreId { get; set; }
    }
}
