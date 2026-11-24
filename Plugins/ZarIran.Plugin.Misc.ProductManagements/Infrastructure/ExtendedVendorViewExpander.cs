using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace ZarIran.Plugin.Misc.ProductManagements.Infrastructure
{
    public class ExtendedVendorViewExapnder : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue(THEME_KEY, out string theme) && context.AreaName == null && context.ViewName == "_ProductBox")
            {
                viewLocations = new string[] {
                                                $"~/Plugins/ZarIran.Misc.ProductManagements/Themes/{theme}/Views/Shared/_ProductBox.cshtml"
                                             }
                .Concat(viewLocations);
            }

            if (context.AreaName == "Admin" && context.ViewName == "CreateWarehouse")
            {
                viewLocations = new string[] {
                                                $"~/Plugins/ZarIran.Misc.ProductManagements/Areas/Admin/Views/Warehouse/CreateWarehouse.cshtml"
                                             }
                .Concat(viewLocations);
            }

            if (context.AreaName == "Admin" && context.ViewName == "EditWarehouse")
            {
                viewLocations = new string[] {
                                                $"~/Plugins/ZarIran.Misc.ProductManagements/Areas/Admin/Views/Warehouse/EditWarehouse.cshtml"
                                             }
                .Concat(viewLocations);
            }

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //do nothing here
        }
    }
}
