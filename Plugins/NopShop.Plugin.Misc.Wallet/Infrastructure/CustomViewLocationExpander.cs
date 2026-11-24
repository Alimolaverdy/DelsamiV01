using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;


namespace NopShop.Plugin.Misc.Wallet.Infrastructure
{
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {

            if (context.Values.TryGetValue(THEME_KEY, out var theme))
            {
                if (context.AreaName == null && context.ViewName == "CustomerRewardPoints")
                    viewLocations = new string[] { $"~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml" }.Concat(viewLocations);
            }
            else
            {
                if (context.AreaName == null && context.ViewName == "CustomerRewardPoints")
                    viewLocations = new string[] { $"~/Plugins/NopShop.Plugin.Wallet/Views/CustomerRewardPoints.cshtml" }.Concat(viewLocations);
            }

            return viewLocations;
        }

        private const string THEME_KEY = "nop.themename";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //no need to add the themeable view locations at all as the administration should not be themeable anyway
            //if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
            //    return;

            //var themeContext = (IThemeContext)context.ActionContext.HttpContext.RequestServices.GetService(typeof(IThemeContext));
            //context.Values[THEME_KEY] = themeContext.WorkingThemeName;
        }
    }
}
