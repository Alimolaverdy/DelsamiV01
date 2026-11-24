using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models
{
    public class ExceptionDateListModel
    {
        public ExceptionDateListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }
        public int? StoreId { get; set; }

        public List<SelectListItem> AvailableStores { get; set; }

        public bool HasAvailableDeliveryTimeRange { get; set; }
    }
}
