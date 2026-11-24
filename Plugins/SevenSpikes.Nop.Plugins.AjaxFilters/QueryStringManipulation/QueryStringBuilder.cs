using System;
using System.Linq;
using System.Text;
using Nop.Web.Framework.UI.Paging;
using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain.Enums;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.AttributeFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.InStockFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.ManufacturerFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.OnSaleFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.PriceRangeFilterSlider;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.SpecificationFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.VendorFilter;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.QueryStringManipulation;

public class QueryStringBuilder : IQueryStringBuilder
{
    private readonly IFiltersPageHelper _filtersPageHelper;

    private SpecificationFilterModel7Spikes _specificationFilterModel7Spikes;

    private AttributeFilterModel7Spikes _attributeFilterModel7Spikes;

    private ManufacturerFilterModel7Spikes _manufacturerFilterModel7Spikes;

    private VendorFilterModel7Spikes _vendorFilterModel7Spikes;

    private OnSaleFilterModel7Spikes _onSaleFilterModel7Spikes;

    private InStockFilterModel7Spikes _inStockFilterModel7Spikes;

    private PriceRangeFilterModel7Spikes _priceRangeFilterModel7Spikes;

    private CatalogProductsCommand _catalogPagingFilteringModel;

    public string SpecificationsQueryString { get; set; }

    public string PagingQueryString { get; set; }

    public string PriceRangeQueryString { get; set; }

    public string ManufacturersQueryString { get; set; }

    public string VendorsQueryString { get; set; }

    public string OnSaleQueryString { get; set; }

    public string InStockQueryString { get; set; }

    public string AttributesQueryString { get; set; }

    public QueryStringBuilder(IFiltersPageHelper filtersPageHelper)
    {
        _filtersPageHelper = filtersPageHelper;
    }

    public string GetQueryString(bool shouldRebuildQueryString)
    {
        if (shouldRebuildQueryString)
        {
            BuildQueryString();
        }
        return GetQueryStringInternal();
    }

    public void SetDataForQueryString(SpecificationFilterModel7Spikes specificationFilterModel7Spikes, AttributeFilterModel7Spikes attributeFilterModel7Spikes, ManufacturerFilterModel7Spikes manufacturerFilterModel7Spikes, VendorFilterModel7Spikes vendorFilterModel7Spikes, PriceRangeFilterModel7Spikes priceRangeFilterModel7Spikes, CatalogProductsCommand catalogPagingFilteringModel, OnSaleFilterModel7Spikes onSaleFilterModel, InStockFilterModel7Spikes inStockFilterModel7Spikes)
    {
        _catalogPagingFilteringModel = catalogPagingFilteringModel;
        _priceRangeFilterModel7Spikes = priceRangeFilterModel7Spikes;
        _manufacturerFilterModel7Spikes = manufacturerFilterModel7Spikes;
        _vendorFilterModel7Spikes = vendorFilterModel7Spikes;
        _onSaleFilterModel7Spikes = onSaleFilterModel;
        _inStockFilterModel7Spikes = inStockFilterModel7Spikes;
        _attributeFilterModel7Spikes = attributeFilterModel7Spikes;
        _specificationFilterModel7Spikes = specificationFilterModel7Spikes;
    }

    private void BuildQueryString()
    {
        BuildSpecificationsQueryString(_specificationFilterModel7Spikes);
        BuildAttributesQueryString(_attributeFilterModel7Spikes);
        BuildManufacturerQueryString(_manufacturerFilterModel7Spikes);
        BuildVendorQueryString(_vendorFilterModel7Spikes);
        BuildOnSaleQueryString(_onSaleFilterModel7Spikes);
        BuildInStockQueryString(_inStockFilterModel7Spikes);
        BuildPriceRangeQueryString(_priceRangeFilterModel7Spikes);
        BuildPagingFilterQueryString(_catalogPagingFilteringModel);
    }

    private void BuildInStockQueryString(InStockFilterModel7Spikes inStockFilterModel7Spikes)
    {
        InStockQueryString = string.Empty;
        if (inStockFilterModel7Spikes != null && (inStockFilterModel7Spikes.FilterItemState == FilterItemState.Checked || inStockFilterModel7Spikes.FilterItemState == FilterItemState.CheckedDisabled))
        {
            StringBuilder itemSb = new StringBuilder();
            itemSb.Append("isFilters=");
            itemSb.Append(inStockFilterModel7Spikes.Id);
            itemSb.Append("!##!");
            TrimEndDelimiter(itemSb, "!##!");
            InStockQueryString = itemSb.ToString();
        }
    }

    private void BuildAttributesQueryString(AttributeFilterModel7Spikes attributeFilterModel7Spikes)
    {
        AttributesQueryString = string.Empty;
        if (attributeFilterModel7Spikes == null || !attributeFilterModel7Spikes.AttributeFilterGroups.SelectMany((AttributeFilterGroup fi) => fi.FilterItems).Any((AttributeFilterItem fis) => fis.FilterItemState == FilterItemState.Checked || fis.FilterItemState == FilterItemState.CheckedDisabled))
        {
            return;
        }
        StringBuilder stringBuilder = new StringBuilder();
        if (attributeFilterModel7Spikes.AttributeFilterGroups.Count > 0)
        {
            stringBuilder.Append("attrFilters=");
        }
        foreach (AttributeFilterGroup attributeFilterGroup in attributeFilterModel7Spikes.AttributeFilterGroups)
        {
            if (stringBuilder.ToString() != "attrFilters=")
            {
                stringBuilder.Append("!-#!");
            }
            StringBuilder itemSb = new StringBuilder();
            foreach (AttributeFilterItem attributeFilterItem in attributeFilterGroup.FilterItems.Where((AttributeFilterItem attrbuteItem) => attrbuteItem.FilterItemState == FilterItemState.Checked || attrbuteItem.FilterItemState == FilterItemState.CheckedDisabled))
            {
                itemSb.Append(attributeFilterItem.ValueId);
                itemSb.Append("!##!");
            }
            TrimEndDelimiter(itemSb, "!##!");
            if (attributeFilterGroup.IsMain && itemSb.Length == 0)
            {
                if (stringBuilder.ToString() != "attrFilters=")
                {
                    stringBuilder.Append("!-#!");
                }
                stringBuilder.Append(attributeFilterGroup.Id);
                stringBuilder.Append("m");
            }
            if (itemSb.Length > 0)
            {
                stringBuilder.Append(attributeFilterGroup.Id);
                if (attributeFilterGroup.IsMain)
                {
                    stringBuilder.Append("m");
                }
                stringBuilder.Append("!#-!");
                stringBuilder.Append(itemSb);
            }
        }
        if (stringBuilder.ToString() != "attrFilters=")
        {
            AttributesQueryString = stringBuilder.ToString();
        }
    }

    private void BuildSpecificationsQueryString(SpecificationFilterModel7Spikes specificationFilterModel7Spikes)
    {
        SpecificationsQueryString = string.Empty;
        if (specificationFilterModel7Spikes == null || !specificationFilterModel7Spikes.SpecificationFilterGroups.SelectMany((SpecificationFilterGroup fi) => fi.FilterItems).Any((SpecificationFilterItem fis) => fis.FilterItemState == FilterItemState.Checked || fis.FilterItemState == FilterItemState.CheckedDisabled))
        {
            return;
        }
        StringBuilder stringBuilder = new StringBuilder();
        if (specificationFilterModel7Spikes.SpecificationFilterGroups.Count > 0)
        {
            stringBuilder.Append("specFilters=");
        }
        foreach (SpecificationFilterGroup specificationFilterGroup in specificationFilterModel7Spikes.SpecificationFilterGroups)
        {
            StringBuilder itemSb = new StringBuilder();
            foreach (SpecificationFilterItem specificationFilterItem in specificationFilterGroup.FilterItems.Where((SpecificationFilterItem specificationItem) => specificationItem.FilterItemState == FilterItemState.Checked || specificationItem.FilterItemState == FilterItemState.CheckedDisabled))
            {
                itemSb.Append(specificationFilterItem.Id);
                itemSb.Append("!##!");
            }
            TrimEndDelimiter(itemSb, "!##!");
            if (specificationFilterGroup.IsMain && itemSb.Length == 0)
            {
                if (stringBuilder.ToString() != "specFilters=")
                {
                    stringBuilder.Append("!-#!");
                }
                stringBuilder.Append(specificationFilterGroup.Id);
                stringBuilder.Append("m");
            }
            if (itemSb.Length > 0)
            {
                if (stringBuilder.ToString() != "specFilters=")
                {
                    stringBuilder.Append("!-#!");
                }
                stringBuilder.Append(specificationFilterGroup.Id);
                if (specificationFilterGroup.IsMain)
                {
                    stringBuilder.Append("m");
                }
                stringBuilder.Append("!#-!");
                stringBuilder.Append(itemSb);
            }
        }
        if (stringBuilder.ToString() != "specFilters=")
        {
            SpecificationsQueryString = stringBuilder.ToString();
        }
    }

    private void BuildPagingFilterQueryString(CatalogProductsCommand pagingFilteringModel)
    {
        PagingQueryString = string.Empty;
        if (pagingFilteringModel == (CatalogProductsCommand)null)
        {
            return;
        }
        StringBuilder sb = new StringBuilder();
        if (((BasePageableModel)pagingFilteringModel).PageSize != _filtersPageHelper.GetDefaultPageSize() || pagingFilteringModel.ViewMode != _filtersPageHelper.GetDefaultViewMode() || pagingFilteringModel.OrderBy != _filtersPageHelper.GetDefaultOrderBy() || ((BasePageableModel)pagingFilteringModel).PageNumber != _filtersPageHelper.GetDefaultPageNumber())
        {
            sb.Append("pageSize=");
            sb.Append(((BasePageableModel)pagingFilteringModel).PageSize);
            if (!string.IsNullOrEmpty(pagingFilteringModel.ViewMode))
            {
                AppendSeparator(sb);
                sb.Append("viewMode=");
                sb.Append(pagingFilteringModel.ViewMode);
            }
            AppendSeparator(sb);
            sb.Append("orderBy=");
            sb.Append(pagingFilteringModel.OrderBy);
            AppendSeparator(sb);
            sb.Append("pageNumber=");
            sb.Append(((BasePageableModel)pagingFilteringModel).PageNumber);
        }
        PagingQueryString = sb.ToString();
    }

    private void BuildPriceRangeQueryString(PriceRangeFilterModel7Spikes priceRangeFilterModel7Spikes)
    {
        PriceRangeQueryString = string.Empty;
        if (priceRangeFilterModel7Spikes == null || priceRangeFilterModel7Spikes.SelectedPriceRange == (PriceRangeModel)null)
        {
            return;
        }
        StringBuilder stringBuilder = new StringBuilder();
        bool hasValue1 = priceRangeFilterModel7Spikes.SelectedPriceRange.From.HasValue;
        bool hasValue2 = priceRangeFilterModel7Spikes.SelectedPriceRange.To.HasValue;
        if (!hasValue1 && !hasValue2)
        {
            return;
        }
        stringBuilder.Append("prFilter=");
        if (hasValue1)
        {
            stringBuilder.Append("From-");
            stringBuilder.Append(Convert.ToInt32(priceRangeFilterModel7Spikes.SelectedPriceRange.From));
        }
        if (hasValue2)
        {
            if (hasValue1)
            {
                stringBuilder.Append("!-#!");
            }
            stringBuilder.Append("To-");
            stringBuilder.Append(Convert.ToInt32(priceRangeFilterModel7Spikes.SelectedPriceRange.To));
        }
        if (stringBuilder.ToString() != "prFilter=")
        {
            PriceRangeQueryString = stringBuilder.ToString();
        }
    }

    private void BuildManufacturerQueryString(ManufacturerFilterModel7Spikes manufacturerFilterModel7Spikes)
    {
        ManufacturersQueryString = string.Empty;
        if (manufacturerFilterModel7Spikes == null || manufacturerFilterModel7Spikes.ManufacturerFilterItems.Count <= 0 || !manufacturerFilterModel7Spikes.ManufacturerFilterItems.Any((ManufacturerFilterItem m) => m.FilterItemState == FilterItemState.Checked || m.FilterItemState == FilterItemState.CheckedDisabled))
        {
            return;
        }
        StringBuilder itemSb = new StringBuilder();
        itemSb.Append("manFilters=");
        foreach (ManufacturerFilterItem manufacturerFilterItem in manufacturerFilterModel7Spikes.ManufacturerFilterItems)
        {
            if (manufacturerFilterItem.FilterItemState == FilterItemState.Checked || manufacturerFilterItem.FilterItemState == FilterItemState.CheckedDisabled)
            {
                itemSb.Append(manufacturerFilterItem.Id);
                itemSb.Append("!##!");
            }
        }
        TrimEndDelimiter(itemSb, "!##!");
        ManufacturersQueryString = itemSb.ToString();
    }

    private void BuildOnSaleQueryString(OnSaleFilterModel7Spikes onSaleFilterModel7Spikes)
    {
        OnSaleQueryString = string.Empty;
        if (onSaleFilterModel7Spikes != null && (onSaleFilterModel7Spikes.FilterItemState == FilterItemState.Checked || onSaleFilterModel7Spikes.FilterItemState == FilterItemState.CheckedDisabled))
        {
            StringBuilder itemSb = new StringBuilder();
            itemSb.Append("osFilters=");
            if (onSaleFilterModel7Spikes.FilterItemState == FilterItemState.Checked || onSaleFilterModel7Spikes.FilterItemState == FilterItemState.CheckedDisabled)
            {
                itemSb.Append(onSaleFilterModel7Spikes.Id);
                itemSb.Append("!##!");
            }
            TrimEndDelimiter(itemSb, "!##!");
            OnSaleQueryString = itemSb.ToString();
        }
    }

    private void BuildVendorQueryString(VendorFilterModel7Spikes vendorFilterModel7Spikes)
    {
        VendorsQueryString = string.Empty;
        if (vendorFilterModel7Spikes == null || vendorFilterModel7Spikes.VendorFilterItems.Count <= 0 || !vendorFilterModel7Spikes.VendorFilterItems.Any((VendorFilterItem v) => v.FilterItemState == FilterItemState.Checked || v.FilterItemState == FilterItemState.CheckedDisabled))
        {
            return;
        }
        StringBuilder itemSb = new StringBuilder();
        itemSb.Append("venFilters=");
        foreach (VendorFilterItem vendorFilterItem in vendorFilterModel7Spikes.VendorFilterItems)
        {
            if (vendorFilterItem.FilterItemState == FilterItemState.Checked || vendorFilterItem.FilterItemState == FilterItemState.CheckedDisabled)
            {
                itemSb.Append(vendorFilterItem.Id);
                itemSb.Append("!##!");
            }
        }
        TrimEndDelimiter(itemSb, "!##!");
        VendorsQueryString = itemSb.ToString();
    }

    private static void AppendSeparator(StringBuilder sb)
    {
        if (sb.Length > 0)
        {
            sb.Append("&");
        }
    }

    private static void AppendQueryString(string queryString, StringBuilder sb)
    {
        if (!string.IsNullOrEmpty(queryString))
        {
            AppendSeparator(sb);
            sb.Append(queryString);
        }
    }

    private static void TrimEndDelimiter(StringBuilder itemSb, string delimeterToBeTrimmed)
    {
        if (itemSb.Length > 0)
        {
            int length = delimeterToBeTrimmed.Length;
            itemSb.Remove(itemSb.Length - length, length);
        }
    }

    private string GetQueryStringInternal()
    {
        StringBuilder sb = new StringBuilder();
        AppendQueryString(SpecificationsQueryString, sb);
        AppendQueryString(AttributesQueryString, sb);
        AppendQueryString(ManufacturersQueryString, sb);
        AppendQueryString(VendorsQueryString, sb);
        AppendQueryString(OnSaleQueryString, sb);
        AppendQueryString(InStockQueryString, sb);
        AppendQueryString(PriceRangeQueryString, sb);
        AppendQueryString(PagingQueryString, sb);
        return sb.ToString();
    }
}
