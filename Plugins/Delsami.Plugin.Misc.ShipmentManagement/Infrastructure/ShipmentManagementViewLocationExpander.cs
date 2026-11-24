using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Infrastructure
{
    public class ShipmentManagementViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin" && context.ViewName.Equals("_OrderDetails.BillingShipping", StringComparison.InvariantCultureIgnoreCase))
            {
                viewLocations = new string[] { $"~/Plugins/ZarIran.Misc.ShipmentManagement/Areas/Admin/Views/Order/_OrderDetails.BillingShipping.cshtml" }.Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
