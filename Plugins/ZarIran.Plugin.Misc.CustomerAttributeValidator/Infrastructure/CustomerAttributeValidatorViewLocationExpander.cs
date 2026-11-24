using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Infrastructure
{
    public class CustomerAttributeValidatorViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue(THEME_KEY, out string theme) && context.AreaName == null && context.ViewName == "Components/CheckoutProgress/Default")
            {
                viewLocations = new string[] {
                                                $"~/Plugins/ZarIran.Misc.CustomerAttributeValidator/Themes/{theme}/Views/Shared/Components/CheckoutProgress/Default.cshtml"
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
