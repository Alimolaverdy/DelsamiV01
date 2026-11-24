using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Localization;

namespace SevenSpikes.Nop.Plugins.JCarousel.Domain;

public static class JCarouselDataSourceTypes
{
    internal const string HomePageFeaturedProducts = "HomePageFeaturedProducts";

    internal const string BestSellersProducts = "BestSellersProducts";

    internal const string BestSellersProductsByQuantity = "BestSellersProductsByQuantity";

    internal const string RecentlyAddedProducts = "RecentlyAddedProducts";

    internal const string MarkedAsNewProducts = "MarkedAsNewProducts";

    internal const string RecentlyViewedProducts = "RecentlyViewedProducts";

    internal const string RelatedProducts = "RelatedProducts";

    internal const string AlsoPurchasedProducts = "AlsoPurchasedProducts";

    internal const string CategoryFeaturedProducts = "CategoryFeaturedProducts";

    internal const string ManufacturerFeaturedProducts = "ManufacturerFeaturedProducts";

    internal const string CrossSellProducts = "CrossSellProducts";

    internal const string None = "None";

    internal const string ProductsFromTheSameCategory = "ProductsFromTheSameCategory";

    internal const string ProductsFromTheSameManufacturer = "ProductsFromTheSameManufacturer";

    internal const string Manufacturers = "Manufacturers";

    internal const string Vendors = "Vendors";

    internal const string ProductsFromTheSameVendor = "ProductsFromTheSameVendor";

    internal const string SiblingCategories = "SiblingCategories";

    internal const string HomePageCategories = "HomePageCategories";

    internal const string SubCategories = "SubCategories";

    public static readonly IList<string> CarouselManufacturerDataSourceTypes = new List<string> { "Manufacturers" }.AsReadOnly();

    public static readonly IList<string> CarouselVendorDataSourceTypes = new List<string> { "Vendors" }.AsReadOnly();

    public static readonly IList<string> CarouselCategoryDataSourceTypes = new List<string> { "SiblingCategories", "HomePageCategories", "SubCategories" }.AsReadOnly();

    public static readonly IList<string> CarouselCategoryAndManufacturerDataSourceTypes = new List<string> { "SiblingCategories", "HomePageCategories", "SubCategories", "Manufacturers" }.AsReadOnly();

    public static async Task<string> GetLocalizedAsync(string jCarouselDataSourceType, ILocalizationService localizationService)
    {
        string text = $"SevenSpikes.JCarousel.Admin.DataSourceTypes.{jCarouselDataSourceType}";
        return await localizationService.GetResourceAsync(text);
    }
}
