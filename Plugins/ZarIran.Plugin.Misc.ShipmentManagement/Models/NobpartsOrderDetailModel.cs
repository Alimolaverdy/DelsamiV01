using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Models
{
    public record ZarIranOrderDetailModel
    {
        public List<ZarIranOrderItemModel> OrderItems { get; set; }
        public ZarIranCustomerInfoModel CustomerInfo { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderTotal { get; set; }
    }
}

