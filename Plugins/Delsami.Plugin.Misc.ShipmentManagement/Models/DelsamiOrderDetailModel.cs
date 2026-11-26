using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ShipmentManagement.Models
{
    public record DelsamiOrderDetailModel
    {
        public List<DelsamiOrderItemModel> OrderItems { get; set; }
        public DelsamiCustomerInfoModel CustomerInfo { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderTotal { get; set; }
    }
}

