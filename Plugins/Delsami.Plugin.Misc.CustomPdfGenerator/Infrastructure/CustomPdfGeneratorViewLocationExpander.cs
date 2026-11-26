using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delsami.Plugin.Misc.CustomPdfGenerator.Infrastructure
{
    public class CustomPdfGeneratorViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {

            if (context.AreaName == "Admin" && context.ViewName.Equals("ShipmentDetails", StringComparison.InvariantCultureIgnoreCase))
            {
                viewLocations = new string[] { $"~/Plugins/Delsami.Misc.CustomPdfGenerator/Areas/Admin/Views/CustomOrder/ShipmentDetails.cshtml" }.Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //do nothing here
        }
    }
}
