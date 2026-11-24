using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using SevenSpikes.Nop.Framework.Mappings;
using SevenSpikes.Nop.Framework.Theme;
using SevenSpikes.Nop.Plugins.JCarousel.Areas.Admin.Extensions;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;
using SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Constants;
using SevenSpikes.Nop.Plugins.JCarousel.Models;
using SevenSpikes.Nop.Plugins.JCarousel.Services;
using ZarIran.Plugin.Misc.ProductManagements.Models;
using static Nop.Web.Models.Catalog.CategoryModel;

namespace SevenSpikes.Nop.Plugins.JCarousel.Components;

[ViewComponent(Name = "JCarousel")]
public class JCarouselComponent : ViewComponent
{
    private readonly IJCarouselService _jCarouselService;
    private readonly IItemsMappingService _itemsMappingService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IAclService _aclService;
    private readonly ICategoryService _categoryService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IVendorService _vendorService;
    private readonly IProductService _productService;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly IPictureService _pictureService;
    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;
    private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
    private readonly IOrderReportService _orderReportService;
    private readonly ISettingService _settingService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IProductModelFactory _productModelFactory;
    private readonly MediaSettings _mediaSettings;
    private readonly VendorSettings _vendorSettings;
    private readonly JCarouselGeneralSettings _jCarouselGeneralSettings;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly ICustomerService _customerService;
    private int _numberOfItems;
    private string _jCarouselDataSource;

    public JCarouselComponent(
        ICategoryService categoryService,
        IManufacturerService manufacturerService,
        IProductService productService,
        IVendorService vendorService,
        IWorkContext workContext,
        IStoreContext storeContext,
        IPictureService pictureService,
        ILocalizationService localizationService,
        IWebHelper webHelper,
        IRecentlyViewedProductsService recentlyViewedProductsService,
        IOrderReportService orderReportService,
        IAclService aclService,
        IStoreMappingService storeMappingService,
        ISettingService settingService,
        IJCarouselService jCarouselService,
        IItemsMappingService itemsMappingService,
        IProductModelFactory productModelFactory,
        IStaticCacheManager staticCacheManager,
        MediaSettings mediaSettings,
        VendorSettings vendorSettings,
        JCarouselGeneralSettings jCarouselGeneralSettings,
        IShoppingCartService shoppingCartService,
        IUrlRecordService urlRecordService,
        ICustomerService customerService)
    {
        _categoryService = categoryService;
        _manufacturerService = manufacturerService;
        _productService = productService;
        _vendorService = vendorService;
        _workContext = workContext;
        _storeContext = storeContext;
        _pictureService = pictureService;
        _localizationService = localizationService;
        _webHelper = webHelper;
        _recentlyViewedProductsService = recentlyViewedProductsService;
        _orderReportService = orderReportService;
        _aclService = aclService;
        _storeMappingService = storeMappingService;
        _settingService = settingService;
        _jCarouselService = jCarouselService;
        _itemsMappingService = itemsMappingService;
        _productModelFactory = productModelFactory;
        _staticCacheManager = staticCacheManager;
        _mediaSettings = mediaSettings;
        _vendorSettings = vendorSettings;
        _jCarouselGeneralSettings = jCarouselGeneralSettings;
        _shoppingCartService = shoppingCartService;
        _urlRecordService = urlRecordService;
        _customerService = customerService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone)
    {
        if (!_jCarouselGeneralSettings.Enable)
            return Content(string.Empty);

        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            CacheKeys.CarouselWidgetByWidgetZoneKey, widgetZone, storeId);

        var carousels = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var result = new List<Carousel>();
            foreach (var carousel in await _jCarouselService.GetCarouselsByWidgetZoneAsync(widgetZone))
            {
                if (await _storeMappingService.AuthorizeAsync(carousel))
                    result.Add(carousel);
            }
            return result;
        });

        if (carousels.Count == 0)
            return Content(string.Empty);

        var models = new List<JCarouselModel>(carousels.Count);
        foreach (var carousel in carousels)
            models.Add(await PrepareJCarouselModelAsync(carousel));

        return models.Count > 0 ? View("JCarousel", models) : Content(string.Empty);
    }

    private void InitializeFields(Carousel carousel)
    {
        _numberOfItems = Math.Max(0, carousel.JCarouselEntitySettings.NumberOfItems);
        _jCarouselDataSource = carousel.DataSourceType;
    }

    private async Task<JCarouselModel> PrepareJCarouselModelAsync(Carousel carousel)
    {
        var model = new JCarouselModel();
        InitializeFields(carousel);
        await PrepareJCarouselProductsModelAsync(carousel, model);
        await PrepareJCarouselSettingsModelAsync(carousel, model);
        model.GeneralSettings = _jCarouselGeneralSettings.ToModel();
        PrepareJCarouselDataSourceClass(model);
        await PrepareJCarouselResponsiveBreakpointsAsync(model);
        await PrepareJCarouselNavigationArrowsSelectorAsync(model);
        return model;
    }

    private void PrepareJCarouselDataSourceClass(JCarouselModel model)
    {
        if (JCarouselDataSourceTypes.CarouselManufacturerDataSourceTypes.Contains(_jCarouselDataSource))
            model.DataSourceTypeClass = "manufacturer-grid";
        else if (JCarouselDataSourceTypes.CarouselCategoryDataSourceTypes.Contains(_jCarouselDataSource))
            model.DataSourceTypeClass = "category-grid";
        else if (JCarouselDataSourceTypes.CarouselVendorDataSourceTypes.Contains(_jCarouselDataSource))
            model.DataSourceTypeClass = "vendor-grid";
        else
            model.DataSourceTypeClass = "product-grid";
    }

    private async Task PrepareJCarouselSettingsModelAsync(Carousel carousel, JCarouselModel model)
    {
        model.Settings = carousel.ToModel();
        model.Settings.CarouselHtmlElementId = GetCarouselHtmlElementId(carousel);
        model.Settings.Title = await _localizationService.GetLocalizedAsync<Carousel, string>(
            carousel, x => x.Title, null, true, true);
        model.Theme = await ThemeHelper.GetPluginThemeAsync("SevenSpikes.Nop.Plugins.JCarousel");
    }

    private async Task PrepareJCarouselResponsiveBreakpointsAsync(JCarouselModel model)
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var themeKey = $"JCarouselGeneralSettings.ResponsiveBreakpointsForSlickSlider-{model.Theme}";
        string sourceKey = null;

        switch (model.DataSourceTypeClass)
        {
            case "category-grid":
                sourceKey = $"JCarouselGeneralSettings.ResponsiveBreakpointsCategorySlider-{model.Theme}";
                break;
            case "manufacturer-grid":
                sourceKey = $"JCarouselGeneralSettings.ResponsiveBreakpointsManufacturerSlider-{model.Theme}";
                break;
            case "vendor-grid":
                sourceKey = $"JCarouselGeneralSettings.ResponsiveBreakpointsVendorSlider-{model.Theme}";
                break;
        }

        var breakpoints = sourceKey == null
            ? string.Empty
            : await _settingService.GetSettingByKeyAsync<string>(sourceKey, "", storeId, true);

        if (string.IsNullOrWhiteSpace(breakpoints))
            breakpoints = await _settingService.GetSettingByKeyAsync<string>(themeKey, "", storeId, true);

        if (string.IsNullOrWhiteSpace(breakpoints))
            breakpoints = await _settingService.GetSettingByKeyAsync<string>(
                "JCarouselGeneralSettings.DefaultResponsiveBreakpointsForSlickSlider", "", storeId, true);

        model.ResponsiveBreakpoints = breakpoints;
    }

    private async Task PrepareJCarouselNavigationArrowsSelectorAsync(JCarouselModel model)
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var themeKey = $"JCarouselGeneralSettings.ArrowsContainer-{model.Theme}";
        var themeSelector = await _settingService.GetSettingByKeyAsync<string>(themeKey, "", storeId, true);
        var defaultSelector = await _settingService.GetSettingByKeyAsync<string>(
            "JCarouselGeneralSettings.DefaultArrowsContainer", "", storeId, true);

        model.NavigationArrowsSelector = string.IsNullOrWhiteSpace(themeSelector)
            ? defaultSelector
            : themeSelector;
    }

    private static string GetCarouselHtmlElementId(Carousel carousel)
    {
        return $"jcarousel-{carousel.Id}-{Guid.NewGuid():N}";
    }

    private async Task PrepareJCarouselProductsModelAsync(Carousel carousel, JCarouselModel model)
    {
        var currentProductId = GetRouteId("productId");
        var currentCategoryId = GetRouteId("categoryId");
        var currentManufacturerId = GetRouteId("manufacturerId");
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer, false);
        var shoppingCartItems = string.Equals(carousel.DataSourceType, "CrossSellProducts", StringComparison.InvariantCultureIgnoreCase)
            ? await _shoppingCartService.GetShoppingCartAsync(customer, (ShoppingCartType)1, 0, null, null, null)
            : (IList<ShoppingCartItem>)new List<ShoppingCartItem>();

        var currencyId = (await _workContext.GetWorkingCurrencyAsync()).Id;
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var isSecure = _webHelper.IsCurrentConnectionSecured();

        // Recently viewed products depend on the current customer and are not cached here.
        if (carousel.DataSourceType == "RecentlyViewedProducts")
        {
            model.Items = await CreateJCarouselProductModelAsync(await GetRecentlyViewedProductsAsync(), carousel);
            _jCarouselDataSource = carousel.DataSourceType;
            return;
        }

        var taxDisplayType = await _workContext.GetTaxDisplayTypeAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
            CacheKeys.CarouselModelCacheKey,
            carousel.Id, customerRoleIds, shoppingCartItems, currentCategoryId,
            currentManufacturerId, currentProductId, currencyId, languageId,
            storeId, isSecure, taxDisplayType);

        var simpleModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            IList<JCarouselItemModel> items = new List<JCarouselItemModel>();

            switch (carousel.DataSourceType)
            {
                case "None":
                    items = await CreateCustomDataSourceAsync(carousel);
                    break;
                case "HomePageFeaturedProducts":
                    items = await CreateJCarouselProductModelAsync(await GetFeaturedProductsAsync(), carousel);
                    break;
                case "RelatedProducts":
                    if (currentProductId != 0)
                        items = await CreateJCarouselProductModelAsync(await GetRelatedProductsAsync(currentProductId), carousel);
                    break;
                case "AlsoPurchasedProducts":
                    if (currentProductId != 0)
                        items = await CreateJCarouselProductModelAsync(await GetProductsAlsoPurchasedAsync(currentProductId), carousel);
                    break;
                case "CategoryFeaturedProducts":
                    if (currentCategoryId != 0)
                        items = await CreateJCarouselProductModelAsync(await GetCategoryFeaturedProductsAsync(currentCategoryId), carousel);
                    break;
                case "RecentlyAddedProducts":
                    items = await CreateJCarouselProductModelAsync(await GetRecentlyAddedProductsAsync(), carousel);
                    break;
                case "MarkedAsNewProducts":
                    items = await CreateJCarouselProductModelAsync(await GetMarkedAsNewProductsAsync(), carousel);
                    break;
                case "BestSellersProducts":
                    items = await CreateJCarouselProductModelAsync(await GetBestSellersProductsAsync(2), carousel);
                    break;
                case "BestSellersProductsByQuantity":
                    items = await CreateJCarouselProductModelAsync(await GetBestSellersProductsAsync(1), carousel);
                    break;
                case "ManufacturerFeaturedProducts":
                    if (currentManufacturerId != 0)
                        items = await CreateJCarouselProductModelAsync(await GetManufacturerFeaturedProductsAsync(currentManufacturerId), carousel);
                    break;
                case "ProductsFromTheSameCategory":
                    if (currentProductId != 0)
                        items = await CreateJCarouselProductModelAsync(await GetProductsFromTheSameCategoryAsync(currentProductId), carousel);
                    break;
                case "ProductsFromTheSameManufacturer":
                    if (currentProductId != 0)
                        items = await CreateJCarouselProductModelAsync(await GetProductsFromTheSameManufacturerAsync(currentProductId), carousel);
                    break;
                case "SiblingCategories":
                    items = await GetSiblingCategoriesAsync(carousel.Id, currentProductId, currentCategoryId);
                    break;
                case "HomePageCategories":
                    items = await GetHomePageCategoriesAsync(carousel.Id);
                    break;
                case "SubCategories":
                    if (currentCategoryId != 0)
                        items = await GetSubCategoriesAsync(currentCategoryId, carousel.Id);
                    break;
                case "Manufacturers":
                    items = CreateJCarouselItemModelForManufacturers(await GetManufacturersAsync(carousel.Id));
                    break;
                case "Vendors":
                    items = CreateJCarouselItemModelForVendors(await GetVendorsAsync());
                    break;
                case "ProductsFromTheSameVendor":
                    items = await CreateJCarouselProductModelAsync(await GetProductsFromTheSameVendor(currentProductId), carousel);
                    break;
                case "CrossSellProducts":
                    items = await CreateJCarouselProductModelAsync(await GetCrossSellProductsAsync(customer), carousel);
                    break;
            }

            if (_numberOfItems > 0)
                items = items.Take(_numberOfItems).ToList();

            return new JCarouselSimpleModel
            {
                Items = items,
                DataSourceType = _jCarouselDataSource
            };
        });

        model.Items = simpleModel.Items;
        _jCarouselDataSource = simpleModel.DataSourceType;
    }

    private int GetRouteId(string key)
    {
        var routeKey = RouteData.Values.Keys.FirstOrDefault(
            routeValueKey => routeValueKey.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (routeKey == null)
            return 0;

        return int.TryParse(Convert.ToString(RouteData.Values[routeKey]), out var id) ? id : 0;
    }

    private async Task<IList<JCarouselItemModel>> CreateJCarouselProductModelAsync(
        IEnumerable<Product> products, Carousel carousel)
    {
        var mappedProducts = await _itemsMappingService.GetProductsMappedToTheJCarouselAsync(carousel.Id);
        var allProducts = products.Concat(mappedProducts).Distinct();
        var imageSize = carousel.JCarouselEntitySettings.ProductImageSize > 0
            ? carousel.JCarouselEntitySettings.ProductImageSize
            : _mediaSettings.ProductThumbPictureSize;

        var overviewModels = await _productModelFactory.PrepareProductOverviewModelsAsync(
            allProducts, true, true, imageSize, false, false);
        return CreateJCarouselProductModel(overviewModels);
    }

    private IList<JCarouselItemModel> CreateJCarouselProductModel(IEnumerable<ProductOverviewModel> products)
    {
        return products.Select(product => new JCarouselItemModel { ProductOverviewModel = (ZarIranProductOverviewModel)product }).ToList();
    }

    private IList<JCarouselItemModel> CreateJCarouselItemModelForManufacturers(IEnumerable<ManufacturerModel> manufacturers)
    {
        return manufacturers.Select(manufacturer => new JCarouselItemModel { ManufacturerModel = manufacturer }).ToList();
    }

    private IList<JCarouselItemModel> CreateJCarouselItemModelForVendors(IEnumerable<VendorModel> vendors)
    {
        return vendors.Select(vendor => new JCarouselItemModel { VendorModel = vendor }).ToList();
    }

    private async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        var products = new List<Product>();
        foreach (var product in await _productService.GetAllProductsDisplayedOnHomepageAsync())
        {
            if (products.Count >= _numberOfItems)
                break;
            if (await _aclService.AuthorizeAsync(product) && await _storeMappingService.AuthorizeAsync(product))
                products.Add(product);
        }
        return products;
    }

    private async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId)
    {
        var related = await _productService.GetRelatedProductsByProductId1Async(productId, false);
        var products = await _productService.GetProductsByIdsAsync(related.Select(x => x.ProductId2).ToArray());
        return await FilterAuthorizedProductsAsync(products);
    }

    private async Task<IEnumerable<Product>> GetProductsAlsoPurchasedAsync(int productId)
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var productIds = await _orderReportService.GetAlsoPurchasedProductsIdsAsync(
            storeId, productId, _numberOfItems, true, false);
        return await FilterAuthorizedProductsAsync(await _productService.GetProductsByIdsAsync(productIds));
    }

    private async Task<IEnumerable<Product>> GetRecentlyViewedProductsAsync()
    {
        var results = new List<Product>();
        foreach (var product in await _recentlyViewedProductsService.GetRecentlyViewedProductsAsync(_numberOfItems))
        {
            if (await _storeMappingService.AuthorizeAsync(product) && await _aclService.AuthorizeAsync(product))
                results.Add(product);
        }
        return results;
    }

    private async Task<IEnumerable<Product>> GetRecentlyAddedProductsAsync()
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        return await _productService.SearchProductsAsync(
            0, _numberOfItems, null, null, storeId, 0, 0, null,
            true, false, null, null, 0, null, false, true, true, false,
            0, null, (ProductSortingEnum)15, false, null);
    }

    private async Task<IEnumerable<Product>> GetMarkedAsNewProductsAsync()
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var pageSize = _numberOfItems > 0 ? _numberOfItems : int.MaxValue;
        return await _productService.GetProductsMarkedAsNewAsync(storeId, 0, pageSize);
    }

    private async Task<IEnumerable<Product>> GetCategoryFeaturedProductsAsync(int categoryId)
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        return await _productService.GetCategoryFeaturedProductsAsync(categoryId, storeId);
    }

    private async Task<IEnumerable<Product>> GetProductsFromTheSameCategoryAsync(int productId)
    {
        var categories = await _categoryService.GetProductCategoriesByProductIdAsync(productId, false);
        var categoryId = categories.Select(x => x.CategoryId).FirstOrDefault();
        if (categoryId == 0)
            return new List<Product>();

        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var searched = await _productService.SearchProductsAsync(
            0, _numberOfItems + 1, new List<int> { categoryId }, null, storeId,
            0, 0, null, false, false, null, null, 0, null, false,
            true, true, false, languageId, null, (ProductSortingEnum)0, false, null);

        var products = new List<Product>();
        foreach (var product in searched)
        {
            if (await _storeMappingService.AuthorizeAsync(product) && product.Id != productId)
                products.Add(product);
        }
        if (products.Count > _numberOfItems)
            products.RemoveAt(products.Count - 1);
        return products;
    }

    private async Task<IEnumerable<Product>> GetProductsFromTheSameManufacturerAsync(int productId)
    {
        if (_numberOfItems <= 0)
            return new List<Product>();

        var manufacturerIds = (await _manufacturerService.GetProductManufacturersByProductIdAsync(productId, false))
            .Select(mapping => mapping.ManufacturerId)
            .Distinct()
            .ToList();

        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var products = new List<Product>(_numberOfItems);
        var seenProductIds = new HashSet<int>();

        foreach (var manufacturerId in manufacturerIds)
        {
            var searched = await _productService.SearchProductsAsync(
                0, _numberOfItems + 1, null, new List<int> { manufacturerId }, storeId,
                0, 0, null, true, false, null, null, 0, null, false,
                true, true, false, languageId, null, (ProductSortingEnum)0, false, null);

            foreach (var product in searched)
            {
                if (product.Id == productId || !seenProductIds.Add(product.Id))
                    continue;

                if (await _storeMappingService.AuthorizeAsync(product))
                    products.Add(product);

                // The previous Distinct().Take(_numberOfItems) returned this same prefix.
                if (products.Count >= _numberOfItems)
                    return products;
            }
        }

        return products;
    }

    private async Task<IEnumerable<Product>> GetBestSellersProductsAsync(int orderBy)
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var report = await _orderReportService.BestSellersReportAsync(
            0, 0, storeId, 0, null, null, null, null, null,
            0, (OrderByEnum)orderBy, 0, _numberOfItems, false);
        var products = await _productService.GetProductsByIdsAsync(report.Select(x => x.ProductId).ToArray());
        return await FilterAuthorizedProductsAsync(products.Where(x => x.VisibleIndividually));
    }

    private async Task<IEnumerable<Product>> GetManufacturerFeaturedProductsAsync(int manufacturerId)
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        return await _productService.GetManufacturerFeaturedProductsAsync(manufacturerId, storeId);
    }

    private async Task<IEnumerable<Product>> GetCrossSellProductsAsync(Customer customer)
    {
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, (ShoppingCartType)1, 0, null, null, null);
        return await _productService.GetCrossSellProductsByShoppingCartAsync(cart, _numberOfItems);
    }

    private async Task<IEnumerable<Product>> FilterAuthorizedProductsAsync(IEnumerable<Product> source)
    {
        var results = new List<Product>();
        foreach (var product in source)
        {
            if (await _aclService.AuthorizeAsync(product) && await _storeMappingService.AuthorizeAsync(product))
                results.Add(product);
        }
        return results;
    }

    private async Task<IEnumerable<ManufacturerModel>> GetManufacturersAsync(int carouselId)
    {
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var manufacturers = (await _manufacturerService.GetAllManufacturersAsync(
            "", storeId, 0, _numberOfItems, false, null)).ToList();
        manufacturers.AddRange(await _itemsMappingService.GetManufacturersMappedToTheJCarouselAsync(carouselId));
        return await PrepareManufacturerModelAsync(manufacturers.Distinct().ToList());
    }

    private async Task<IEnumerable<VendorModel>> GetVendorsAsync()
    {
        var vendors = (await _vendorService.GetAllVendorsAsync("", "", 0, _numberOfItems, false)).ToList();
        return await PrepareVendorModelAsync(vendors.Distinct().ToList());
    }

    private async Task<IEnumerable<Product>> GetProductsFromTheSameVendor(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null)
            return new List<Product>();

        var vendor = await _vendorService.GetVendorByIdAsync(product.VendorId);
        if (vendor == null)
            return new List<Product>();

        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var searched = await _productService.SearchProductsAsync(
            0, _numberOfItems + 1, null, null, storeId, product.VendorId,
            0, null, false, false, null, null, 0, null, false,
            true, true, false, languageId, null, (ProductSortingEnum)0, false, null);

        var products = new List<Product>();
        foreach (var item in searched)
        {
            if (await _storeMappingService.AuthorizeAsync(item) && item.Id != productId)
                products.Add(item);
        }
        if (products.Count > _numberOfItems)
            products.RemoveAt(products.Count - 1);
        return products;
    }

    private async Task<IList<JCarouselItemModel>> GetSiblingCategoriesAsync(
        int carouselId, int productId = 0, int categoryId = 0)
    {
        if (productId == 0 && categoryId == 0)
            return new List<JCarouselItemModel>();

        if (categoryId == 0)
        {
            var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(productId, false))
                .FirstOrDefault();
            if (productCategory == null)
                return new List<JCarouselItemModel>();
            categoryId = productCategory.CategoryId;
        }

        var category = await _categoryService.GetCategoryByIdAsync(categoryId);
        var siblings = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(category.ParentCategoryId, false);
        var categories = new List<Category>();
        foreach (var sibling in siblings)
        {
            if (sibling.Id != categoryId && await _storeMappingService.AuthorizeAsync(sibling))
                categories.Add(sibling);
        }
        categories.AddRange(await _itemsMappingService.GetCategoriesMappedToTheJCarouselAsync(carouselId));

        var results = new List<JCarouselItemModel>();
        foreach (var sibling in categories.Distinct())
        {
            var name = await _localizationService.GetLocalizedAsync<Category, string>(
                sibling, x => x.Name, null, true, true);
            var model = new JCarouselItemModel
            {
                CategoryModel = new SubCategoryModel
                {
                    Id = sibling.Id,
                    Name = name,
                    SeName = await _urlRecordService.GetSeNameAsync<Category>(sibling, null, true, true)
                }
            };
            model.CategoryModel.PictureModel = await GetCachedPictureModelAsync(
                sibling.Id, sibling.PictureId, _mediaSettings.CategoryThumbPictureSize,
                NopModelCacheDefaults.CategoryPictureModelKey,
                "Media.Category.ImageLinkTitleFormat", "Media.Category.ImageAlternateTextFormat", name);
            results.Add(model);
        }
        return results;
    }

    private async Task<IList<JCarouselItemModel>> GetHomePageCategoriesAsync(int carouselId)
    {
        var categories = new List<Category>();
        foreach (var category in await _categoryService.GetAllCategoriesDisplayedOnHomepageAsync(false))
        {
            if (await _storeMappingService.AuthorizeAsync(category))
                categories.Add(category);
        }
        categories.AddRange(await _itemsMappingService.GetCategoriesMappedToTheJCarouselAsync(carouselId));

        var results = new List<JCarouselItemModel>();
        foreach (var category in categories.Distinct())
        {
            if (!await _aclService.AuthorizeAsync(category))
                continue;

            var categoryModel = await category.ToModelAsync();
            var item = new JCarouselItemModel
            {
                CategoryModel = new SubCategoryModel
                {
                    Id = categoryModel.Id,
                    Name = categoryModel.Name,
                    SeName = categoryModel.SeName,
                    Description = categoryModel.Description,
                    PictureModel = categoryModel.PictureModel
                }
            };
            item.CategoryModel.PictureModel = await GetCachedPictureModelAsync(
                category.Id, category.PictureId, _mediaSettings.CategoryThumbPictureSize,
                NopModelCacheDefaults.CategoryPictureModelKey,
                "Media.Category.ImageLinkTitleFormat", "Media.Category.ImageAlternateTextFormat", categoryModel.Name);
            results.Add(item);
        }
        return results;
    }

    private async Task<IList<JCarouselItemModel>> GetSubCategoriesAsync(int categoryId, int carouselId)
    {
        var categories = new List<Category>();
        foreach (var category in await _categoryService.GetAllCategoriesByParentCategoryIdAsync(categoryId, false))
        {
            if (await _storeMappingService.AuthorizeAsync(category))
                categories.Add(category);
        }
        categories.AddRange(await _itemsMappingService.GetCategoriesMappedToTheJCarouselAsync(carouselId));
        return await PrepareJCarouselItemModelForCategoriesAsync(categories.Distinct());
    }

    private async Task<IList<JCarouselItemModel>> CreateCustomDataSourceAsync(Carousel carousel)
    {
        var products = await _itemsMappingService.GetProductsMappedToTheJCarouselAsync(carousel.Id);
        if (products.Count > 0)
        {
            _jCarouselDataSource = "HomePageFeaturedProducts";
            return await CreateJCarouselProductModelAsync(products, carousel);
        }

        var categories = await _itemsMappingService.GetCategoriesMappedToTheJCarouselAsync(carousel.Id);
        if (categories.Count > 0)
        {
            _jCarouselDataSource = "HomePageCategories";
            return await PrepareJCarouselItemModelForCategoriesAsync(categories);
        }

        var manufacturers = await _itemsMappingService.GetManufacturersMappedToTheJCarouselAsync(carousel.Id);
        if (manufacturers.Count > 0)
        {
            _jCarouselDataSource = "Manufacturers";
            return CreateJCarouselItemModelForManufacturers(await PrepareManufacturerModelAsync(manufacturers));
        }

        var vendors = await _itemsMappingService.GetVendorsMappedToTheJCarouselAsync(carousel.Id);
        if (vendors.Count > 0)
        {
            _jCarouselDataSource = "Vendors";
            return CreateJCarouselItemModelForVendors(await PrepareVendorModelAsync(vendors));
        }

        return new List<JCarouselItemModel>();
    }

    private async Task<IList<JCarouselItemModel>> PrepareJCarouselItemModelForCategoriesAsync(
        IEnumerable<Category> categories)
    {
        var results = new List<JCarouselItemModel>();
        foreach (var category in categories)
        {
            var name = await _localizationService.GetLocalizedAsync<Category, string>(
                category, x => x.Name, null, true, true);
            var model = new JCarouselItemModel
            {
                CategoryModel = new SubCategoryModel
                {
                    Id = category.Id,
                    Name = name,
                    SeName = await _urlRecordService.GetSeNameAsync<Category>(category, null, true, true)
                }
            };
            model.CategoryModel.PictureModel = await CreatePictureModelAsync(
                category.PictureId, _mediaSettings.CategoryThumbPictureSize,
                "Media.Category.ImageLinkTitleFormat", "Media.Category.ImageAlternateTextFormat", name);
            results.Add(model);
        }
        return results;
    }

    private async Task<IEnumerable<ManufacturerModel>> PrepareManufacturerModelAsync(IList<Manufacturer> manufacturers)
    {
        var results = new List<ManufacturerModel>();
        for (var i = 0; i < manufacturers.Count && i < _numberOfItems; i++)
        {
            var manufacturer = manufacturers[i];
            var model = await manufacturer.ToModel();
            model.PictureModel = await GetCachedPictureModelAsync(
                manufacturer.Id, manufacturer.PictureId, _mediaSettings.ManufacturerThumbPictureSize,
                NopModelCacheDefaults.ManufacturerPictureModelKey,
                "Media.Manufacturer.ImageLinkTitleFormat", "Media.Manufacturer.ImageAlternateTextFormat", model.Name);
            results.Add(model);
        }
        return results;
    }

    private async Task<IEnumerable<VendorModel>> PrepareVendorModelAsync(IList<Vendor> vendors)
    {
        var results = new List<VendorModel>();
        for (var i = 0; i < vendors.Count && i < _numberOfItems; i++)
        {
            var vendor = vendors[i];
            var model = new VendorModel
            {
                Id = vendor.Id,
                Name = await _localizationService.GetLocalizedAsync<Vendor, string>(vendor, x => x.Name, null, true, true),
                Description = await _localizationService.GetLocalizedAsync<Vendor, string>(vendor, x => x.Description, null, true, true),
                MetaKeywords = await _localizationService.GetLocalizedAsync<Vendor, string>(vendor, x => x.MetaKeywords, null, true, true),
                MetaDescription = await _localizationService.GetLocalizedAsync<Vendor, string>(vendor, x => x.MetaDescription, null, true, true),
                MetaTitle = await _localizationService.GetLocalizedAsync<Vendor, string>(vendor, x => x.MetaTitle, null, true, true),
                SeName = await _urlRecordService.GetSeNameAsync<Vendor>(vendor, null, true, true),
                AllowCustomersToContactVendors = _vendorSettings.AllowCustomersToContactVendors
            };
            model.PictureModel = await GetCachedPictureModelAsync(
                vendor.Id, vendor.PictureId, _mediaSettings.VendorThumbPictureSize,
                NopModelCacheDefaults.VendorPictureModelKey,
                "Media.Vendor.ImageLinkTitleFormat", "Media.Vendor.ImageAlternateTextFormat", model.Name);
            results.Add(model);
        }
        return results;
    }

    private async Task<PictureModel> GetCachedPictureModelAsync(
        int entityId, int pictureId, int pictureSize, CacheKey cacheKey,
        string titleResourceKey, string altResourceKey, string name)
    {
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
        var key = _staticCacheManager.PrepareKeyForDefaultCache(
            cacheKey, entityId, pictureSize, true, languageId,
            _webHelper.IsCurrentConnectionSecured(), storeId);

        return await _staticCacheManager.GetAsync(key, () =>
            CreatePictureModelAsync(pictureId, pictureSize, titleResourceKey, altResourceKey, name));
    }

    private async Task<PictureModel> CreatePictureModelAsync(
        int pictureId, int pictureSize, string titleResourceKey, string altResourceKey, string name)
    {
        return new PictureModel
        {
            FullSizeImageUrl = await _pictureService.GetPictureUrlAsync(pictureId, 0, true, null, (PictureType)1),
            ImageUrl = await _pictureService.GetPictureUrlAsync(pictureId, pictureSize, true, null, (PictureType)1),
            Title = string.Format(await _localizationService.GetResourceAsync(titleResourceKey), name),
            AlternateText = string.Format(await _localizationService.GetResourceAsync(altResourceKey), name)
        };
    }
}
