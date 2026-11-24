using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record ZarIranOrderSearchModel : OrderSearchModel
    {
        public IList<SelectListItem> AvailableShippingMethod { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.ShippingMethod")]
        public int ShippingMethodId { get; set; }

        public ZarIranOrderSearchModel()
        {
            AvailableShippingMethod = new List<SelectListItem>();
        }
    }
}
