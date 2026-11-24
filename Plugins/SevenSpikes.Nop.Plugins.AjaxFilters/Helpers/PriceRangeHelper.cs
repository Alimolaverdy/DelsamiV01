using System.Globalization;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Web.Models.Catalog;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public static class PriceRangeHelper
{
    public static PriceRangeModel GetSelectedPriceRange(decimal priceRangeTollerance = 0m)
    {
        string str = EngineContext.Current.Resolve<IWebHelper>().QueryString<string>("price");
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        string[] strArray = str.Trim().Split('-');
        if (strArray.Length == 2)
        {
            CultureInfo specificCulture = CultureInfo.CreateSpecificCulture("en-us");
            decimal.TryParse(strArray[0].Trim(), NumberStyles.Number, specificCulture, out var result1);
            decimal.TryParse(strArray[1].Trim(), NumberStyles.Number, specificCulture, out var result2);
            if (result1 != 0m || result2 != 0m)
            {
                return new PriceRangeModel
                {
                    From = result1 - priceRangeTollerance,
                    To = result2 + priceRangeTollerance
                };
            }
        }
        return null;
    }
}
