using Nop.Web.Models.Catalog;
using SevenSpikes.Nop.Framework.Models;
using SevenSpikes.Nop.Plugins.AjaxFilters.Areas.Admin.Models;
using System.Collections.Generic;
using System.Linq;

public record ProductsModel : Base7SpikesProductsModel
{
    public IEnumerable<ProductOverviewModel> Products { get; set; }

    public IList<int> ProductIdsToDetermineFilters { get; set; }

    public string SpecificationFilterModel7SpikesJson { get; set; }

    public string AttributeFilterModel7SpikesJson { get; set; }

    public string ManufacturerFilterModel7SpikesJson { get; set; }

    public string VendorFilterModel7SpikesJson { get; set; }

    public string OnSaleFilterModel7SpikesJson { get; set; }

    public string InStockFilterModel7SpikesJson { get; set; }

    public string ViewMode { get; set; }

    public CatalogProductsCommand PagingFilteringContext { get; set; }

    public NopAjaxFiltersSettingsModel NopAjaxFiltersSettingsModel { get; set; }

    public string HashQuery { get; set; }

    public string PriceRangeFromJson { get; set; }

    public string PriceRangeToJson { get; set; }

    public string CurrentPageSizeJson { get; set; }

    public string CurrentViewModeJson { get; set; }

    public string CurrentOrderByJson { get; set; }

    public string CurrentPageNumberJson { get; set; }

    public int TotalCount { get; set; }

    public ProductsModel()
    {
        Products = new List<ProductOverviewModel>();
        PagingFilteringContext = new CatalogProductsCommand();
    }

    public override IList<ProductOverviewModel> GetProductOverviewModels(string consumerName)
    {
        return Products.ToList();
    }

    protected ProductsModel(ProductsModel original)
        : base((Base7SpikesProductsModel)original)
    {
        Products = original.Products;
        ProductIdsToDetermineFilters = original.ProductIdsToDetermineFilters;
        SpecificationFilterModel7SpikesJson = original.SpecificationFilterModel7SpikesJson;
        AttributeFilterModel7SpikesJson = original.AttributeFilterModel7SpikesJson;
        ManufacturerFilterModel7SpikesJson = original.ManufacturerFilterModel7SpikesJson;
        VendorFilterModel7SpikesJson = original.VendorFilterModel7SpikesJson;
        OnSaleFilterModel7SpikesJson = original.OnSaleFilterModel7SpikesJson;
        InStockFilterModel7SpikesJson = original.InStockFilterModel7SpikesJson;
        ViewMode = original.ViewMode;
        PagingFilteringContext = original.PagingFilteringContext;
        NopAjaxFiltersSettingsModel = original.NopAjaxFiltersSettingsModel;
        HashQuery = original.HashQuery;
        PriceRangeFromJson = original.PriceRangeFromJson;
        PriceRangeToJson = original.PriceRangeToJson;
        CurrentPageSizeJson = original.CurrentPageSizeJson;
        CurrentViewModeJson = original.CurrentViewModeJson;
        CurrentOrderByJson = original.CurrentOrderByJson;
        CurrentPageNumberJson = original.CurrentPageNumberJson;
        TotalCount = original.TotalCount;
    }
}
