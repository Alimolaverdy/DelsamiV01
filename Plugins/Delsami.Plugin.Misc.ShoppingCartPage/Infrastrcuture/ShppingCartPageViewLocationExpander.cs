using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delsami.Plugin.Misc.ShoppingCartPage.Infrastrcuture
{
    public class ShppingCartPageViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue(THEME_KEY, out string theme) && context.AreaName == null && context.ViewName.Equals("Components/OrderSummary/Default", StringComparison.InvariantCultureIgnoreCase))
            {
                viewLocations = new string[] { $"~/Plugins/Delsami.Misc.ShoppingCartPage/Themes/{theme}/Views/Shared/Components/OrderSummary/Default.cshtml" }.Concat(viewLocations);
            }

            if (context.Values.TryGetValue(THEME_KEY, out string theme2) && context.AreaName == null && context.ViewName.Equals("_AddToCart", StringComparison.InvariantCultureIgnoreCase))
            {
                viewLocations = new string[] { $"~/Plugins/Delsami.Misc.ShoppingCartPage/Views/Product/_AddToCart.cshtml" }.Concat(viewLocations);
            }

            if (context.Values.TryGetValue(THEME_KEY, out string theme3) && context.AreaName == null && context.ViewName.Equals("_OrderReviewData", StringComparison.InvariantCultureIgnoreCase))
            {
                viewLocations = new string[] { $"~/Plugins/Delsami.Misc.ShoppingCartPage/Themes/Traction/Views/Shared/_OrderReviewData.cshtml" }.Concat(viewLocations);
            }
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //do nothing here
        }
    }
}
