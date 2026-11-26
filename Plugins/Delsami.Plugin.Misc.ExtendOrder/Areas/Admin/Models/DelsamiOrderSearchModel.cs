using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record DelsamiOrderSearchModel : OrderSearchModel
    {
        public IList<SelectListItem> AvailableShippingMethod { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.ShippingMethod")]
        public int ShippingMethodId { get; set; }

        public DelsamiOrderSearchModel()
        {
            AvailableShippingMethod = new List<SelectListItem>();
        }
    }
}
