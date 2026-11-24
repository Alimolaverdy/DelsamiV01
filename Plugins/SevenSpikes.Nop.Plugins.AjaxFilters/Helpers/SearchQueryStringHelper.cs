using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Models.Catalog;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public class SearchQueryStringHelper : ISearchQueryStringHelper
{
    public SearchQueryStringParameters GetQueryStringParameters(string queryString)
    {
        if (string.IsNullOrEmpty(queryString))
        {
            return new SearchQueryStringParameters();
        }
        SearchQueryStringParameters stringParameters = new SearchQueryStringParameters();
        Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(queryString);
        if (!query.ContainsKey("q"))
        {
            return stringParameters;
        }
        stringParameters.IsOnSearchPage = true;
        string str1 = query["q"];
        if (!string.IsNullOrEmpty(str1))
        {
            stringParameters.Keyword = str1.Trim();
        }
        if (query.ContainsKey("advs"))
        {
            string str2 = query["advs"];
            if (!string.IsNullOrEmpty(str2) && bool.TryParse(str2.Split(',')[0], out var result8))
            {
                stringParameters.AdvancedSearch = result8;
            }
        }
        if (stringParameters.AdvancedSearch)
        {
            if (query.ContainsKey("Sid"))
            {
                string str3 = query["Sid"];
                if (!string.IsNullOrEmpty(str3) && bool.TryParse(str3.Split(',')[0], out var result7))
                {
                    stringParameters.SearchInProductDescriptions = result7;
                }
            }
            if (query.ContainsKey("Isc"))
            {
                string str4 = query["Isc"];
                if (!string.IsNullOrEmpty(str4) && bool.TryParse(str4.Split(',')[0], out var result6))
                {
                    stringParameters.IncludeSubcategories = result6;
                }
            }
            if (query.ContainsKey("Cid"))
            {
                string s5 = query["Cid"];
                if (!string.IsNullOrEmpty(s5) && int.TryParse(s5, out var result5))
                {
                    stringParameters.SearchCategoryId = result5;
                }
            }
            if (query.ContainsKey("Mid"))
            {
                string s4 = query["Mid"];
                if (!string.IsNullOrEmpty(s4) && int.TryParse(s4, out var result4))
                {
                    stringParameters.SearchManufacturerId = result4;
                }
            }
            if (query.ContainsKey("Vid"))
            {
                string s3 = query["Vid"];
                if (!string.IsNullOrEmpty(s3) && int.TryParse(s3, out var result3))
                {
                    stringParameters.SearchVendorId = result3;
                }
            }
            if (query.ContainsKey("Pf"))
            {
                string s2 = query["Pf"];
                if (!string.IsNullOrEmpty(s2))
                {
                    stringParameters.PriceFrom = (decimal.TryParse(s2, out var result2) ? new decimal?(result2) : null);
                }
            }
            if (query.ContainsKey("Pt"))
            {
                string s = query["Pt"];
                if (!string.IsNullOrEmpty(s))
                {
                    stringParameters.PriceTo = (decimal.TryParse(s, out var result) ? new decimal?(result) : null);
                }
            }
        }
        return stringParameters;
    }

    public RouteValueDictionary PrepareSearchRouteValues(SearchModel model, CatalogProductsCommand command)
    {
        return new RouteValueDictionary(new
        {
            model.q,
            model.advs,
            model.cid,
            model.sid,
            model.isc,
            model.mid,
            model.CatalogProductsModel.PriceRangeFilter.SelectedPriceRange.From,
            model.CatalogProductsModel.PriceRangeFilter.SelectedPriceRange.To,
            ((BasePageableModel)command).PageIndex,
            ((BasePageableModel)command).PageNumber,
            ((BasePageableModel)command).PageSize
        });
    }
}
