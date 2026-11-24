using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using SevenSpikes.Nop.Plugins.AjaxFilters.Domain;
using SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Catalog.DTO;
using SevenSpikes.Nop.Services.Helpers;

public class ProductServiceNopAjaxFilters : IProductServiceNopAjaxFilters
{

    private readonly ILanguageService _languageService;
    private readonly INopDataProvider _dataProvider;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly ICategoryService7Spikes _categoryService7Spikes;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
    private readonly IRepository<Manufacturer> _manufacturerRepository;
    private readonly IRepository<Vendor> _vendorRepository;
    private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventory;
    private readonly CommonSettings _commonSettings;
    private readonly CatalogSettings _catalogSettings;
    private readonly NopAjaxFiltersSettings _nopAjaxFiltersSettings;
    private readonly IProductService _productService;
    private IStaticCacheManager _cacheManager;
    private IAclHelper _aclHelper;
    private readonly ICustomerService _customerService;
    private CacheKey GROUPPRODUCTIDS_BY_CATEGORYIDS => new CacheKey("nop.groupproductids.categoryids.{0}", Array.Empty<string>());
    private CacheKey GROUPPRODUCTIDS_BY_MANUFACTURERID => new CacheKey("nop.groupproductids.manufacturerid.{0}", Array.Empty<string>());
    private CacheKey GROUPPRODUCTIDS_BY_VENDORID => new CacheKey("nop.groupproductids.vendorid.{0}", Array.Empty<string>());
    private CacheKey ONSALESTATE_BY_PRODUCTIDS_KEY => new CacheKey("Nop.onsalestate.productids-{0}-{1}-{2}", Array.Empty<string>());
    private CacheKey INSTOCKSTATE_BY_PRODUCTIDS_KEY => new CacheKey("nop.instock.productids-{0}-{1}-{2}", Array.Empty<string>());

    public ProductServiceNopAjaxFilters(ILanguageService languageService,
                                        INopDataProvider dataProvider,
                                        ICategoryService7Spikes categoryService7Spikes,
                                        IRepository<Product> productRepository,
                                        IRepository<ProductManufacturer> productManufacturer,
                                        IRepository<Manufacturer> manufacturerRepository,
                                        IRepository<Vendor> vendorRepository,
                                        IRepository<ProductCategory> productCategoryRepository,
                                        IRepository<ProductWarehouseInventory> productWarehouseInventory,
                                        IWorkContext workContext,
                                        IStoreContext storeContext,
                                        CommonSettings commonSettings,
                                        CatalogSettings catalogSettings,
                                        NopAjaxFiltersSettings nopAjaxFiltersSettings,
                                        IStaticCacheManager cacheManager,
                                        IAclHelper aclHelper,
                                        ICustomerService customerService,
                                        IProductService productSearchService)
    {
        _languageService = languageService;
        _dataProvider = dataProvider;
        _categoryService7Spikes = categoryService7Spikes;
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _productManufacturerRepository = productManufacturer;
        _manufacturerRepository = manufacturerRepository;
        _vendorRepository = vendorRepository;
        _productWarehouseInventory = productWarehouseInventory;
        _workContext = workContext;
        _storeContext = storeContext;
        _commonSettings = commonSettings;
        _cacheManager = cacheManager;
        _aclHelper = aclHelper;
        _nopAjaxFiltersSettings = nopAjaxFiltersSettings;
        _catalogSettings = catalogSettings;
        _customerService = customerService;
        _productService = productSearchService;
    }

    public virtual async Task<ProductsResultDataDTO> SearchProductsAsync(IList<int> categoryIds, SpecificationFilterModelDTO specifiationFilterModelDTO, AttributeFilterModelDTO attributeFilterModelDTO, ManufacturerFilterModelDTO manufacturerFilterModelDTO, VendorFilterModelDTO vendorFilterModelDTO, int pageIndex = 0, int pageSize = int.MaxValue, int manufacturerId = 0, int vendorId = 0, int storeId = 0, bool? featuredProducts = null, decimal? priceMin = null, decimal? priceMax = null, int productTagId = 0, string keywords = null, bool searchDescriptions = false, bool searchSku = false, bool searchProductTags = false, int languageId = 0, ProductSortingEnum orderBy = 0, bool showHidden = false, bool onSale = false, bool inStock = false)
    {
        return await SearchProductsInternalAsync(categoryIds, specifiationFilterModelDTO, attributeFilterModelDTO, manufacturerFilterModelDTO, vendorFilterModelDTO, pageIndex, pageSize, manufacturerId, vendorId, storeId, featuredProducts, priceMin, priceMax, productTagId, keywords, searchDescriptions, searchSku, searchProductTags, languageId, orderBy, showHidden, onSale, inStock);
    }

    public async Task<bool> HasProductsOnSaleAsync(int categoryId, int manufacturerId, int vendorId)
    {
        return await HasProductsOnSaleInternalAsync(categoryId, manufacturerId, vendorId);
    }

    public async Task<bool> HasProductsInStockAsync(int categoryId, int manufacturerId, int vendorId)
    {
        return await HasProductsInStockInternalAsync(categoryId, manufacturerId, vendorId);
    }

    public async Task<IList<int>> GetAllGroupProductIdsInCategoriesAsync(List<int> categoriesIds)
    {
        return (await GetAllGroupProductIdsInCategoriesInternalAsync(categoriesIds)).ToList();
    }

    public async Task<IList<int>> GetAllGroupProductIdsInCategoryAsync(int categoryId)
    {
        return (await GetAllGroupProductIdsInCategoriesInternalAsync(new List<int> { categoryId })).ToList();
    }

    public async Task<IList<int>> GetAllGroupProductIdsInManufacturerAsync(int manufacturerId)
    {
        return (await GetAllGroupProductIdsInManufacturerInternalAsync(manufacturerId)).ToList();
    }

    public async Task<IList<int>> GetAllGroupProductIdsInVendorAsync(int vendorId)
    {
        return (await GetAllGroupProductIdsInVendorInternalAsync(vendorId)).ToList();
    }

    public async Task<bool> HasProductsOnSaleInternalAsync(int categoryId, int manufacturerId, int vendorId)
    {
        CacheKey val = _cacheManager.PrepareKeyForDefaultCache(ONSALESTATE_BY_PRODUCTIDS_KEY, new object[3] { categoryId, manufacturerId, vendorId });
        return await _cacheManager.GetAsync<bool>(val, (Func<Task<bool>>)async delegate
        {
            bool hasProducts = false;
            IQueryable<Product> availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
            bool showProductsFromSubcategories = _catalogSettings.ShowProductsFromSubcategories;
            bool includeFeaturedProducts = _catalogSettings.IncludeFeaturedProductsInNormalLists;
            if (categoryId > 0)
            {
                List<int> categoryIds = new List<int> { categoryId };
                if (showProductsFromSubcategories)
                {
                    IEnumerable<int> collection = (await _categoryService7Spikes.GetCategoriesByParentCategoryIdAsync(categoryId, true, false)).Select((Category x) => x.Id);
                    categoryIds.AddRange(collection);
                }
                IList<int> groupProductIds = await GetAllGroupProductIdsInCategoriesInternalAsync(categoryIds);
                hasProducts = await HasAvailableProductsOnSaleInCategoryAsync(availableProducts, groupProductIds, categoryIds, includeFeaturedProducts);
            }
            else if (manufacturerId > 0)
            {
                IList<int> groupProductIds2 = await GetAllGroupProductIdsInManufacturerInternalAsync(manufacturerId);
                hasProducts = await HasAvailableProductsOnSaleInManufacturerAsync(availableProducts, groupProductIds2, manufacturerId, includeFeaturedProducts);
            }
            else if (vendorId > 0)
            {
                IList<int> groupProductIds3 = await GetAllGroupProductIdsInVendorInternalAsync(vendorId);
                hasProducts = await HasAvailableProductsOnSaleInVendorAsync(availableProducts, groupProductIds3, vendorId, includeFeaturedProducts);
            }
            return hasProducts;
        });
    }

    public async Task<bool> HasProductsInStockInternalAsync(int categoryId, int manufacturerId, int vendorId)
    {
        CacheKey val = _cacheManager.PrepareKeyForDefaultCache(INSTOCKSTATE_BY_PRODUCTIDS_KEY, new object[3] { categoryId, manufacturerId, vendorId });
        return await _cacheManager.GetAsync<bool>(val, (Func<Task<bool>>)async delegate
        {
            bool hasProducts = false;
            IQueryable<Product> availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
            bool showProductsFromSubcategories = _catalogSettings.ShowProductsFromSubcategories;
            bool includeFeaturedProducts = _catalogSettings.IncludeFeaturedProductsInNormalLists;
            if (categoryId > 0)
            {
                List<int> categoryIds = new List<int> { categoryId };
                if (showProductsFromSubcategories)
                {
                    IEnumerable<int> collection = (await _categoryService7Spikes.GetCategoriesByParentCategoryIdAsync(categoryId, true, false)).Select((Category x) => x.Id);
                    categoryIds.AddRange(collection);
                }
                IList<int> groupProductIds = await GetAllGroupProductIdsInCategoriesInternalAsync(categoryIds);
                hasProducts = await HasAvailableProductsInStockInCategoryAsync(availableProducts, groupProductIds, categoryIds, includeFeaturedProducts);
            }
            else if (manufacturerId > 0)
            {
                IList<int> groupProductIds2 = await GetAllGroupProductIdsInManufacturerInternalAsync(manufacturerId);
                hasProducts = await HasAvailableProductsInStockInManufacturerAsync(availableProducts, groupProductIds2, manufacturerId, includeFeaturedProducts);
            }
            else if (vendorId > 0)
            {
                IList<int> groupProductIds3 = await GetAllGroupProductIdsInVendorInternalAsync(vendorId);
                hasProducts = await HasAvailableProductsInStockInVendorAsync(availableProducts, groupProductIds3, vendorId, includeFeaturedProducts);
            }
            return hasProducts;
        });
    }

    private async Task<ProductsResultDataDTO> SearchProductsInternalAsync(IList<int> categoryIds,
                                                                          SpecificationFilterModelDTO specifiationFilterModelDTO,
                                                                          AttributeFilterModelDTO attributeFilterModelDTO,
                                                                          ManufacturerFilterModelDTO manufacturerFilterModelDTO,
                                                                          VendorFilterModelDTO vendorFilterModelDTO,
                                                                          int pageIndex = 0,
                                                                          int pageSize = int.MaxValue,
                                                                          int manufacturerId = 0,
                                                                          int vendorId = 0,
                                                                          int storeId = 0,
                                                                          bool? featuredProducts = null,
                                                                          decimal? priceMin = null,
                                                                          decimal? priceMax = null,
                                                                          int productTagId = 0,
                                                                          string keywords = null,
                                                                          bool searchDescriptions = false,
                                                                          bool searchSku = false,
                                                                          bool searchProductTags = false,
                                                                          int languageId = 0,
                                                                          ProductSortingEnum orderBy = 0,
                                                                          bool showHidden = false,
                                                                          bool onSale = false,
                                                                          bool inStock = false)
    {

        if (!string.IsNullOrWhiteSpace(keywords))
        {
            var productSearchResult = await _productService.SearchProductsAsync(
                categoryIds: categoryIds,
                keywords: keywords,
                productType: ProductType.GroupedProduct,
                visibleIndividuallyOnly: true,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var products = await _productRepository.GetByIdsAsync(productSearchResult.Select(s => s.Id).ToList());

            PagedList<Product> productsPagedList = new PagedList<Product>(products, pageIndex, pageSize, (int)productSearchResult.TotalCount);
            List<int> specificationOptionIds = new();
            List<int> productVariantIds = new();
            List<int> manufacturerIds = new();
            List<int> vendorIds = new();

            return new ProductsResultDataDTO
            {
                ProductsPagedList = (IPagedList<Product>)productsPagedList,
                SpecificationOptionIds = specificationOptionIds,
                ProductVariantIds = productVariantIds,
                ManufacturerIds = manufacturerIds,
                VendorIds = vendorIds,
            };
        }
        else
        {
            if (pageSize == 6)
                pageSize = 24;
            bool searchLocalizedValue = false;
            if (languageId > 0)
            {
                if (showHidden)
                {
                    searchLocalizedValue = true;
                }
                else
                {
                    ILanguageService languageService = _languageService;
                    ILanguageService val = languageService;
                    int count = (await val.GetAllLanguagesAsync(false, ((BaseEntity)(await _storeContext.GetCurrentStoreAsync())).Id)).Count;
                    searchLocalizedValue = count >= 2;
                }
            }
            if (categoryIds != null && categoryIds.Contains(0))
            {
                categoryIds.Remove(0);
            }
            ICustomerService customerService = _customerService;
            ICustomerService val2 = customerService;
            int[] values = await val2.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync(), false);
            string text = (_catalogSettings.IgnoreAcl ? string.Empty : string.Join(",", values));
            string parameterValue = ((categoryIds == null) ? string.Empty : string.Join(",", categoryIds));
            string text2 = FormatSortedIds(
                specifiationFilterModelDTO?.SpecificationFilterDTOs?
                    .SelectMany(filter => filter.SelectedFilterIds ?? Enumerable.Empty<int>()));
            string text3 = FormatSortedIds(
                attributeFilterModelDTO?.AttributeFilterDTOs?
                    .SelectMany(filter => filter.SelectedProductVariantIds ?? Enumerable.Empty<int>()));
            string text4 = FormatSortedIds(manufacturerFilterModelDTO?.SelectedFilterIds);
            string text5 = FormatSortedIds(vendorFilterModelDTO?.SelectedFilterIds);
            if (pageSize == int.MaxValue)
            {
                pageSize = 2147483646;
            }
            DataParameter stringParameter = SqlParameterHelper.GetStringParameter("CategoryIds", parameterValue);
            DataParameter int32Parameter = SqlParameterHelper.GetInt32Parameter("ManufacturerId", manufacturerId);
            DataParameter int32Parameter4 = SqlParameterHelper.GetInt32Parameter("StoreId", (!_catalogSettings.IgnoreStoreLimitations) ? storeId : 0);
            DataParameter int32Parameter5 = SqlParameterHelper.GetInt32Parameter("VendorId", vendorId);
            DataParameter int32Parameter6 = SqlParameterHelper.GetInt32Parameter("ParentGroupedProductId", 0);
            DataParameter int32Parameter7 = SqlParameterHelper.GetInt32Parameter("ProductTypeId", null);
            DataParameter booleanParameter = SqlParameterHelper.GetBooleanParameter("VisibleIndividuallyOnly", true);
            DataParameter int32Parameter8 = SqlParameterHelper.GetInt32Parameter("ProductTagId", productTagId);
            DataParameter booleanParameter6 = SqlParameterHelper.GetBooleanParameter("FeaturedProducts", featuredProducts);
            DataParameter decimalParameter = SqlParameterHelper.GetDecimalParameter("PriceMin", priceMin);
            DataParameter decimalParameter2 = SqlParameterHelper.GetDecimalParameter("PriceMax", priceMax);
            DataParameter stringParameter2 = SqlParameterHelper.GetStringParameter("Keywords", keywords);
            DataParameter booleanParameter7 = SqlParameterHelper.GetBooleanParameter("SearchDescriptions", searchDescriptions);
            DataParameter booleanParameter8 = SqlParameterHelper.GetBooleanParameter("SearchManufacturerPartNumber", true);
            DataParameter booleanParameter9 = SqlParameterHelper.GetBooleanParameter("SearchSku", searchSku);
            DataParameter booleanParameter10 = SqlParameterHelper.GetBooleanParameter("SearchProductTags", searchProductTags);
            DataParameter booleanParameter11 = SqlParameterHelper.GetBooleanParameter("UseFullTextSearch", false);
            DataParameter int32Parameter9 = SqlParameterHelper.GetInt32Parameter("FullTextMode", 0);
            DataParameter stringParameter3 = SqlParameterHelper.GetStringParameter("FilteredSpecs", text2);
            DataParameter stringParameter4 = SqlParameterHelper.GetStringParameter("FilteredProductVariantAttributes", text3);
            DataParameter stringParameter5 = SqlParameterHelper.GetStringParameter("FilteredManufacturers", text4);
            DataParameter stringParameter6 = SqlParameterHelper.GetStringParameter("FilteredVendors", text5);
            DataParameter booleanParameter12 = SqlParameterHelper.GetBooleanParameter("OnSale", onSale);
            DataParameter booleanParameter13 = SqlParameterHelper.GetBooleanParameter("InStock", inStock);
            DataParameter int32Parameter10 = SqlParameterHelper.GetInt32Parameter("LanguageId", searchLocalizedValue ? languageId : 0);
            DataParameter int32Parameter11 = SqlParameterHelper.GetInt32Parameter("OrderBy", (int)orderBy);
            DataParameter stringParameter7 = SqlParameterHelper.GetStringParameter("AllowedCustomerRoleIds", (!_catalogSettings.IgnoreAcl) ? text : string.Empty);
            DataParameter int32Parameter2 = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            DataParameter int32Parameter3 = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);
            DataParameter booleanParameter2 = SqlParameterHelper.GetBooleanParameter("ShowHidden", showHidden);
            DataParameter booleanParameter3 = SqlParameterHelper.GetBooleanParameter("LoadAvailableFilters", true);
            DataParameter pFilterableSpecificationAttributeOptionIds = SqlParameterHelper.GetOutputStringParameter("FilterableSpecificationAttributeOptionIds");
            pFilterableSpecificationAttributeOptionIds.Size = 2147483646;
            DataParameter pFilterableProductVariantAttributeIds = SqlParameterHelper.GetOutputStringParameter("FilterableProductVariantAttributeIds");
            pFilterableProductVariantAttributeIds.Size = 2147483646;
            DataParameter pFilterableManufacturerIds = SqlParameterHelper.GetOutputStringParameter("FilterableManufacturerIds");
            pFilterableManufacturerIds.Size = 2147483646;
            DataParameter pFilterableVendorIds = SqlParameterHelper.GetOutputStringParameter("FilterableVendorIds");
            pFilterableVendorIds.Size = 2147483646;
            DataParameter booleanParameter4 = SqlParameterHelper.GetBooleanParameter("IsOnSaleFilterEnabled", _nopAjaxFiltersSettings.EnableOnSaleFilter);
            DataParameter booleanParameter5 = SqlParameterHelper.GetBooleanParameter("IsInStockFilterEnabled", _nopAjaxFiltersSettings.EnableInStockFilter);
            DataParameter pHasProductsOnSale = new DataParameter
            {
                Name = "HasProductsOnSale",
                DataType = (DataType)11,
                Direction = ParameterDirection.Output
            };
            DataParameter pHasProductsInStock = new DataParameter
            {
                Name = "HasProductsInStock",
                DataType = (DataType)11,
                Direction = ParameterDirection.Output
            };
            DataParameter pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");
            List<Product> list5 = (await _dataProvider.QueryProcAsync<Product>("ProductLoadAllPagedNopAjaxFilters", (DataParameter[])new DataParameter[40]
            {
                stringParameter, int32Parameter, int32Parameter4, int32Parameter5, int32Parameter6, int32Parameter7, booleanParameter, int32Parameter8, booleanParameter6, decimalParameter,
                decimalParameter2, stringParameter2, booleanParameter7, booleanParameter8, booleanParameter9, booleanParameter10, booleanParameter11, int32Parameter9, stringParameter3, stringParameter4,
                stringParameter5, stringParameter6, booleanParameter12, booleanParameter13, int32Parameter10, int32Parameter11, stringParameter7, int32Parameter2, int32Parameter3, booleanParameter2,
                booleanParameter3, pFilterableSpecificationAttributeOptionIds, pFilterableProductVariantAttributeIds, pFilterableManufacturerIds, pFilterableVendorIds, booleanParameter4, booleanParameter5, pHasProductsOnSale, pHasProductsInStock, pTotalRecords
            })).ToList();
            List<int> specificationOptionIds = new List<int>();
            string text6 = ((pFilterableSpecificationAttributeOptionIds.Value != DBNull.Value) ? ((string)pFilterableSpecificationAttributeOptionIds.Value) : "");
            if (!string.IsNullOrWhiteSpace(text6))
            {
                specificationOptionIds = (from x in text6.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          select Convert.ToInt32(x.Trim())).ToList();
            }
            List<int> productVariantIds = new List<int>();
            string text7 = ((pFilterableProductVariantAttributeIds.Value != DBNull.Value) ? ((string)pFilterableProductVariantAttributeIds.Value) : "");
            if (!string.IsNullOrWhiteSpace(text7))
            {
                productVariantIds = (from x in text7.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     select Convert.ToInt32(x.Trim())).ToList();
            }
            List<int> manufacturerIds = new List<int>();
            string text8 = ((pFilterableManufacturerIds.Value != DBNull.Value) ? ((string)pFilterableManufacturerIds.Value) : "");
            if (!string.IsNullOrWhiteSpace(text8))
            {
                manufacturerIds = (from x in text8.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   select Convert.ToInt32(x.Trim())).ToList();
            }
            List<int> vendorIds = new List<int>();
            string text9 = ((pFilterableVendorIds.Value != DBNull.Value) ? ((string)pFilterableVendorIds.Value) : "");
            if (!string.IsNullOrWhiteSpace(text9))
            {
                vendorIds = (from x in text9.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             select Convert.ToInt32(x.Trim())).ToList();
            }
            int value = ((pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0);
            bool hasProductsOnSale = pHasProductsOnSale.Value != DBNull.Value && Convert.ToBoolean(pHasProductsOnSale.Value);
            bool hasProductsInStock = pHasProductsInStock.Value != DBNull.Value && Convert.ToBoolean(pHasProductsInStock.Value);
            PagedList<Product> productsPagedList = new PagedList<Product>((IList<Product>)list5, pageIndex, pageSize, (int?)value);
            return new ProductsResultDataDTO
            {
                ProductsPagedList = (IPagedList<Product>)productsPagedList,
                SpecificationOptionIds = specificationOptionIds,
                ProductVariantIds = productVariantIds,
                ManufacturerIds = manufacturerIds,
                VendorIds = vendorIds,
                HasProductsOnSale = hasProductsOnSale,
                HasProductsInStock = hasProductsInStock
            };
        }

    }

    private async Task<bool> HasAvailableProductsInStockInVendorAsync(
        IQueryable<Product> availableProducts,
        IList<int> groupProductIds,
        int vendorId,
        bool includeFeaturedProducts)
    {
        var activeVendors = _vendorRepository.Table.Where(vendor => vendor.Active && !vendor.Deleted);
        var query = from product in availableProducts
                    join vendor in activeVendors
                        on product.VendorId equals vendor.Id into vendors
                    from vendor in vendors.DefaultIfEmpty()
                    where (
                              vendor != null
                              && vendor.Id == vendorId
                              && product.ProductTypeId != 10
                              && product.VisibleIndividually
                          )
                          || groupProductIds.Contains(product.ParentGroupedProductId)
                    where product.Published
                          && !product.Deleted
                          && (
                              product.ManageInventoryMethodId == 0
                              || (product.ManageInventoryMethodId == 1
                                  && (
                                      (product.StockQuantity > 0 && !product.UseMultipleWarehouses)
                                      || (product.UseMultipleWarehouses
                                          && _productWarehouseInventory.Table.Any(inventory =>
                                              inventory.ProductId == product.Id
                                              && inventory.StockQuantity > 0
                                              && inventory.StockQuantity > inventory.ReservedQuantity))
                                  ))
                          )
                    select product;

        return await AsyncIQueryableExtensions.AnyAsync<Product>(
            query,
            (Expression<Func<Product, bool>>)null);
    }

    private async Task<bool> HasAvailableProductsInStockInManufacturerAsync(
        IQueryable<Product> availableProducts,
        IList<int> groupProductIds,
        int manufacturerId,
        bool includeFeaturedProducts)
    {
        var publishedManufacturerMappings =
            from productManufacturer in _productManufacturerRepository.Table
            join manufacturer in _manufacturerRepository.Table
                on productManufacturer.ManufacturerId equals manufacturer.Id
            where manufacturer.Published && !manufacturer.Deleted
            select productManufacturer;

        var query = from product in availableProducts
                    join productManufacturer in publishedManufacturerMappings
                        on product.Id equals productManufacturer.ProductId into productManufacturers
                    from productManufacturer in productManufacturers.DefaultIfEmpty()
                    where (
                              productManufacturer != null
                              && productManufacturer.ManufacturerId == manufacturerId
                              && product.ProductTypeId != 10
                              && product.VisibleIndividually
                              && (productManufacturer.IsFeaturedProduct == includeFeaturedProducts
                                  || !productManufacturer.IsFeaturedProduct)
                          )
                          || (
                              groupProductIds.Contains(product.ParentGroupedProductId)
                              && product.Published
                              && !product.Deleted
                              && (
                                  product.ManageInventoryMethodId == 0
                                  || (product.ManageInventoryMethodId == 1
                                      && (
                                          (product.StockQuantity > 0 && !product.UseMultipleWarehouses)
                                          || (product.UseMultipleWarehouses
                                              && _productWarehouseInventory.Table.Any(inventory =>
                                                  inventory.ProductId == product.Id
                                                  && inventory.StockQuantity > 0
                                                  && inventory.StockQuantity > inventory.ReservedQuantity))
                                      ))
                              )
                          )
                    select product;

        return await AsyncIQueryableExtensions.AnyAsync<Product>(
            query,
            (Expression<Func<Product, bool>>)null);
    }

    private async Task<bool> HasAvailableProductsInStockInCategoryAsync(
        IQueryable<Product> availableProducts,
        IList<int> groupProductIds,
        List<int> categoryIds,
        bool includeFeaturedProducts)
    {
        var query = from product in availableProducts
                    join productCategory in _productCategoryRepository.Table
                        on product.Id equals productCategory.ProductId into productCategories
                    from productCategory in productCategories.DefaultIfEmpty()
                    where (
                              productCategory != null
                              && categoryIds.Contains(productCategory.CategoryId)
                              && product.ProductTypeId != 10
                              && product.VisibleIndividually
                              && (productCategory.IsFeaturedProduct == includeFeaturedProducts
                                  || !productCategory.IsFeaturedProduct)
                          )
                          || groupProductIds.Contains(product.ParentGroupedProductId)
                    where product.Published
                          && !product.Deleted
                          && (
                              product.ManageInventoryMethodId == 0
                              || (product.ManageInventoryMethodId == 1
                                  && (
                                      (product.StockQuantity > 0 && !product.UseMultipleWarehouses)
                                      || (product.UseMultipleWarehouses
                                          && _productWarehouseInventory.Table.Any(inventory =>
                                              inventory.ProductId == product.Id
                                              && inventory.StockQuantity > 0
                                              && inventory.StockQuantity > inventory.ReservedQuantity))
                                  ))
                          )
                    select product;

        return await AsyncIQueryableExtensions.AnyAsync<Product>(
            query,
            (Expression<Func<Product, bool>>)null);
    }

    private async Task<IList<int>> GetAllGroupProductIdsInCategoriesInternalAsync(List<int> categoryIds)
    {
        // Treat category IDs as a set so equivalent filters share the same cache entry.
        var normalizedCategoryIds = categoryIds?.Distinct().OrderBy(id => id).ToList()
            ?? new List<int>();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            GROUPPRODUCTIDS_BY_CATEGORYIDS,
            normalizedCategoryIds);

        return await _cacheManager.GetAsync<List<int>>(
            cacheKey,
            async () =>
            {
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                var groupedProducts = availableProducts
                    .Where(product => !product.Deleted && product.Published && product.ProductTypeId == 10);

                if (normalizedCategoryIds.Count > 0)
                {
                    groupedProducts = from product in groupedProducts
                                      join productCategory in _productCategoryRepository.Table
                                          on product.Id equals productCategory.ProductId
                                      where normalizedCategoryIds.Contains(productCategory.CategoryId)
                                      select product;
                }

                return await AsyncIQueryableExtensions.ToListAsync(
                    groupedProducts.Select(product => product.Id).Distinct());
            });
    }

    private async Task<IList<int>> GetAllGroupProductIdsInManufacturerInternalAsync(int manufacturerId)
    {
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            GROUPPRODUCTIDS_BY_MANUFACTURERID,
            manufacturerId);

        return await _cacheManager.GetAsync<List<int>>(
            cacheKey,
            async () =>
            {
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                var query = from product in availableProducts
                            join productManufacturer in _productManufacturerRepository.Table
                                on product.Id equals productManufacturer.ProductId
                            where !product.Deleted
                                  && product.Published
                                  && product.ProductTypeId == 10
                                  && productManufacturer.ManufacturerId == manufacturerId
                            select product.Id;

                return await AsyncIQueryableExtensions.ToListAsync(query);
            });
    }

    private async Task<IList<int>> GetAllGroupProductIdsInVendorInternalAsync(int vendorId)
    {
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            GROUPPRODUCTIDS_BY_VENDORID,
            vendorId);

        return await _cacheManager.GetAsync<List<int>>(
            cacheKey,
            async () =>
            {
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                var query = availableProducts
                    .Where(product => !product.Deleted
                                      && product.Published
                                      && product.ProductTypeId == 10
                                      && product.VendorId == vendorId)
                    .Select(product => product.Id);

                return await AsyncIQueryableExtensions.ToListAsync(query);
            });
    }

    private async Task<bool> HasAvailableProductsOnSaleInCategoryAsync(
        IQueryable<Product> availableProducts,
        IList<int> groupProductIds,
        IList<int> categoryIds,
        bool includeFeaturedProducts)
    {
        var nowUtc = DateTime.UtcNow;
        var query = from product in availableProducts
                    join productCategory in _productCategoryRepository.Table
                        on product.Id equals productCategory.ProductId into productCategories
                    from productCategory in productCategories.DefaultIfEmpty()
                    where (
                              productCategory != null
                              && categoryIds.Contains(productCategory.CategoryId)
                              && product.ProductTypeId != 10
                              && product.VisibleIndividually
                              && (productCategory.IsFeaturedProduct == includeFeaturedProducts
                                  || !productCategory.IsFeaturedProduct)
                          )
                          || groupProductIds.Contains(product.ParentGroupedProductId)
                    where product.Published
                          && !product.Deleted
                          && (!product.AvailableStartDateTimeUtc.HasValue
                              || product.AvailableStartDateTimeUtc <= nowUtc)
                          && (!product.AvailableEndDateTimeUtc.HasValue
                              || product.AvailableEndDateTimeUtc >= nowUtc)
                          && product.OldPrice > 0m
                          && product.OldPrice != product.Price
                    select product;

        return await AsyncIQueryableExtensions.AnyAsync<Product>(
            query,
            (Expression<Func<Product, bool>>)null);
    }

    private async Task<bool> HasAvailableProductsOnSaleInVendorAsync(
        IQueryable<Product> availableProducts,
        IList<int> groupProductIds,
        int vendorId,
        bool includeFeaturedProducts)
    {
        var nowUtc = DateTime.UtcNow;
        var activeVendors = _vendorRepository.Table.Where(vendor => vendor.Active && !vendor.Deleted);
        var query = from product in availableProducts
                    join vendor in activeVendors
                        on product.VendorId equals vendor.Id into vendors
                    from vendor in vendors.DefaultIfEmpty()
                    where (
                              vendor != null
                              && vendor.Id == vendorId
                              && product.ProductTypeId != 10
                              && product.VisibleIndividually
                          )
                          || groupProductIds.Contains(product.ParentGroupedProductId)
                    where product.Published
                          && !product.Deleted
                          && (!product.AvailableStartDateTimeUtc.HasValue
                              || product.AvailableStartDateTimeUtc <= nowUtc)
                          && (!product.AvailableEndDateTimeUtc.HasValue
                              || product.AvailableEndDateTimeUtc >= nowUtc)
                          && product.OldPrice > 0m
                          && product.OldPrice != product.Price
                    select product;

        return await AsyncIQueryableExtensions.AnyAsync<Product>(
            query,
            (Expression<Func<Product, bool>>)null);
    }

    private async Task<bool> HasAvailableProductsOnSaleInManufacturerAsync(
        IQueryable<Product> availableProducts,
        IList<int> groupProductIds,
        int manufacturerId,
        bool includeFeaturedProducts)
    {
        var nowUtc = DateTime.UtcNow;
        var publishedManufacturerMappings =
            from productManufacturer in _productManufacturerRepository.Table
            join manufacturer in _manufacturerRepository.Table
                on productManufacturer.ManufacturerId equals manufacturer.Id
            where manufacturer.Published && !manufacturer.Deleted
            select productManufacturer;

        var query = from product in availableProducts
                    join productManufacturer in publishedManufacturerMappings
                        on product.Id equals productManufacturer.ProductId into productManufacturers
                    from productManufacturer in productManufacturers.DefaultIfEmpty()
                    where (
                              productManufacturer != null
                              && productManufacturer.ManufacturerId == manufacturerId
                              && product.ProductTypeId != 10
                              && product.VisibleIndividually
                              && (productManufacturer.IsFeaturedProduct == includeFeaturedProducts
                                  || !productManufacturer.IsFeaturedProduct)
                          )
                          || groupProductIds.Contains(product.ParentGroupedProductId)
                    where product.Published
                          && !product.Deleted
                          && (!product.AvailableStartDateTimeUtc.HasValue
                              || product.AvailableStartDateTimeUtc <= nowUtc)
                          && (!product.AvailableEndDateTimeUtc.HasValue
                              || product.AvailableEndDateTimeUtc >= nowUtc)
                          && product.OldPrice > 0m
                          && product.OldPrice != product.Price
                    select product;

        return await AsyncIQueryableExtensions.AnyAsync<Product>(
            query,
            (Expression<Func<Product, bool>>)null);
    }

    private static string FormatSortedIds(IEnumerable<int> ids)
    {
        return ids == null
            ? string.Empty
            : string.Join(",", ids.OrderBy(id => id));
    }

}
