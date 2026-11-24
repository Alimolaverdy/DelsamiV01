using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Catalog.DTO;
using SevenSpikes.Nop.Services.Helpers;

public class ProductAttributeService7Spikes : IProductAttributeService7Spikes
{
    private readonly IRepository<ProductManufacturer> _productManufacturerRepository;

    private readonly IRepository<Vendor> _vendorRepository;

    private readonly IAclHelper _aclHelper;

    private readonly IStoreContext _storeContext;

    private readonly IWorkContext _workContext;

    private readonly IProductServiceNopAjaxFilters _productServiceNopAjaxFilters;

    private readonly ILanguageService _languageService;

    private IStoreHelper _storeHelper;

    private CacheKey PRODUCTATTRIBUTES_BY_CATEGORYID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattribute.categoryid-{0}.includeproductsinsubcategories-{1}.showhidden-{2}.store.id-{3}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTATTRIBUTES_BY_MANUFACTURERID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattribute.manufacturerid-{0}.showhidden-{1}.store.id-{2}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTATTRIBUTES_BY_VENDORID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattribute.vendorid-{0}.showhidden-{1}.store.id-{2}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTATTRIBUTES_BY_PRODUCTVARIANTIDS_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattribute.productvariantids-{0}.showhidden-{1}.store.id-{2}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTATTRIBUTEVALUES_BY_PRODUCTATTRIBUTEID_AND_CATEGORYID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattributevalues.productattributeid-{0}.categoryid-{1}.includeproductsinsubcategories-{2}.showhidden-{3}.store.id-{4}.language.id-{5}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTATTRIBUTEVALUES_BY_PRODUCTATTRIBUTEID_AND_MANUFACTURERID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattributevalues.productattributeid-{0}.manufacturerid-{1}.showhidden-{2}.store.id-{3}.language.id-{4}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTATTRIBUTEVALUES_BY_PRODUCTATTRIBUTEID_AND_VENDORID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattributevalues.productattributeid-{0}.vendorid-{1}.showhidden-{2}.store.id-{3}.language.id-{4}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTATTRIBUTEVALUES_AND_PRODUCT_ATTRIBUTES_BY_PRODUCTATTRIBUTEMAPPINGS_KEY => new CacheKey("nop.pres.nop.ajax.filters.productattributevalues.productattributemappingids-{0}.store.id-{1}.language.id-{2}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTVARIANTATTRIBUTES_BY_PRODUCTID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productvariantattribute.productid-{0}.showhidden-{1}.store.id-{2}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTVARIANTATTRIBUTES_WHICH_HAVE_VALUES_BY_PRODUCTID_KEY => new CacheKey("nop.pres.nop.ajax.filters.productvariantattributeswhichhavevalues.productid-{0}.showhidden-{1}.store.id-{2}", new[] { "nop.pres.nop.ajax.filters" });

    private CacheKey PRODUCTVARIANTATTRIBUTES_WHICH_HAVE_VALUES_BY_PRODUCTIDS_KEY => new CacheKey("nop.pres.nop.ajax.filters.productvariantattributeswhichhavevalues.productids-{0}.showhidden-{1}.store.id-{2}", new[] { "nop.pres.nop.ajax.filters" });

    private IRepository<ProductAttribute> ProductAttributeRepository { get; set; }

    private IRepository<ProductAttributeMapping> ProductAttributeMappingRepository { get; set; }

    private IRepository<ProductAttributeValue> ProductAttributeValueRepository { get; set; }

    private IRepository<ProductCategory> ProductCategoryRepository { get; set; }

    private IRepository<LocalizedProperty> LocalizedPropertyRepository { get; set; }

    private ICategoryService7Spikes CategoryService7Spikes { get; set; }

    private ISettingService SettingService { get; set; }

    private IStaticCacheManager CacheManager { get; set; }

    public ProductAttributeService7Spikes(IRepository<ProductManufacturer> productManufacturerRepository, IRepository<Vendor> vendorRepository, IRepository<ProductAttribute> productAttributeRepository, IRepository<ProductAttributeMapping> productVariantAttributeRepository, IRepository<ProductAttributeValue> productVariantAttributeValueRepository, IRepository<ProductCategory> productCategoryRepository, ICategoryService7Spikes categoryService7Spikes, ISettingService settingService, IStaticCacheManager cacheManager, IAclHelper aclHelper, IStoreContext storeContext, IProductServiceNopAjaxFilters productServiceNopAjaxFilters, IStoreHelper storeHelper, IRepository<LocalizedProperty> localizedProperty, ILanguageService languageService, IWorkContext workContext)
    {
        _productManufacturerRepository = productManufacturerRepository;
        _vendorRepository = vendorRepository;
        _aclHelper = aclHelper;
        _storeContext = storeContext;
        _workContext = workContext;
        _productServiceNopAjaxFilters = productServiceNopAjaxFilters;
        _storeHelper = storeHelper;
        _languageService = languageService;
        ProductAttributeRepository = productAttributeRepository;
        ProductAttributeMappingRepository = productVariantAttributeRepository;
        ProductAttributeValueRepository = productVariantAttributeValueRepository;
        ProductCategoryRepository = productCategoryRepository;
        LocalizedPropertyRepository = localizedProperty;
        CategoryService7Spikes = categoryService7Spikes;
        SettingService = settingService;
        CacheManager = cacheManager;
    }

    public virtual async Task<IList<ProductAttribute>> GetAllProductAttributesByCategoryIdAsync(int categoryId, bool includeProductsInSubcategories = false, bool showHiddenProducts = false)
    {
        return await GetAllProductAttributesByCategoryIdInternalAsync(categoryId, includeProductsInSubcategories, showHiddenProducts);
    }

    public async Task<IList<ProductAttribute>> GetAllProductAttributesByManufacturerIdAsync(int manufacturerId, bool showHiddenProducts = false)
    {
        return await GetAllProductAttributesByManufacturerIdInternalAsync(manufacturerId, showHiddenProducts);
    }

    public async Task<IList<ProductAttribute>> GetAllProductAttributesByVendorIdAsync(int vendorId, bool showHiddenProducts = false)
    {
        return await GetAllProductAttributesByVendorIdInternalAsync(vendorId, showHiddenProducts);
    }

    public async Task<IList<ProductAttribute>> GetAllProductAttributesByProductAttributeMappingIdsAsync(IList<int> productAttributeMappingIds, bool showHiddenProducts = false)
    {
        return await GetAllProductAttributesByProductAttributeMappingIdsInternalAsync(productAttributeMappingIds, showHiddenProducts);
    }

    public virtual async Task<IList<ProductAttributeValue>> GetAllProductVariantAttributeValuesByProductAttributeIdAndCategoryIdAsync(int productAttributeId, int categoryId, bool includeProductsInSubcategories = false, bool showHiddenProducts = false)
    {
        return await GetAllProductVariantAttributeValuesByProductAttributeIdAndCategoryIdInternalAsync(productAttributeId, categoryId, includeProductsInSubcategories, showHiddenProducts);
    }

    public async Task<IList<ProductAttributeValue>> GetAllProductVariantAttributeValuesByProductAttributeIdAndManufacturerIdAsync(int productAttributeId, int manufacturerId, bool showHiddenProducts = false)
    {
        return await GetAllProductVariantAttributeValuesByProductAttributeIdAndManufacturerIdInternalAsync(productAttributeId, manufacturerId, showHiddenProducts);
    }

    public async Task<IList<ProductAttributeValue>> GetAllProductVariantAttributeValuesByProductAttributeIdAndVendorIdAsync(int productAttributeId, int vendorId, bool showHiddenProducts = false)
    {
        return await GetAllProductVariantAttributeValuesByProductAttributeIdAndVendorIdInternalAsync(productAttributeId, vendorId, showHiddenProducts);
    }

    public virtual async Task<IList<ProductAttributeMapping>> GetAllProductVariantAttributesByProductIdAsync(int productId, bool showHiddenProducts = false)
    {
        return await GetAllProductVariantAttributesByProductIdInternalAsync(productId, showHiddenProducts);
    }

    public virtual async Task<IList<ProductAttributeMapping>> GetAllProductVariantAttributesWhichHaveValuesByProductIdAsync(int productId, bool showHiddenProducts = false)
    {
        return await GetAllProductVariantAttributesWhichHaveValuesByProductIdInternalAsync(productId, showHiddenProducts);
    }

    public virtual async Task<IList<ProductAttributeMapping>> GetAllProductVariantAttributesWhichHaveValuesByProductIdsAsync(IList<int> productIds, bool showHiddenProducts = false)
    {
        return await GetAllProductVariantAttributesWhichHaveValuesByProductIdsInternalAsync(productIds, showHiddenProducts);
    }

    public async Task<IList<ProductAttributeProductAttributeValueDTO>> GetProductAttributeProductAttributeValueDtosByProductAttributeMappingIdsAsync(IList<int> productAttributeMappingIds)
    {
        return await GetProductAttributeProductAttributeValueDtosByProductAttributeMappingIdsInternalAsync(productAttributeMappingIds);
    }

    private async Task<IList<ProductAttribute>> GetAllProductAttributesByCategoryIdInternalAsync(
        int categoryId,
        bool includeProductsInSubcategories,
        bool showHiddenProducts)
    {
        if (categoryId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTES_BY_CATEGORYID_KEY,
            categoryId,
            includeProductsInSubcategories,
            showHiddenProducts,
            store.Id);

        return new List<ProductAttribute>(await CacheManager.GetAsync<IEnumerable<ProductAttribute>>(
            cacheKey,
            async () =>
            {
                var categoryIds = new List<int> { categoryId };
                if (includeProductsInSubcategories)
                {
                    var childCategoryIds = await CategoryService7Spikes
                        .GetCategoryIdsByParentCategoryAsync(categoryId, false);
                    categoryIds.AddRange(childCategoryIds);
                }

                var groupProductIds = await _productServiceNopAjaxFilters
                    .GetAllGroupProductIdsInCategoriesAsync(categoryIds);
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);
                var includeFeaturedProducts = await GetIncludeFeaturedProductsInNormalListAsync();

                var query = from productAttribute in ProductAttributeRepository.Table
                            join productAttributeMapping in ProductAttributeMappingRepository.Table
                                on productAttribute.Id equals productAttributeMapping.ProductAttributeId
                            join product in availableProducts
                                on productAttributeMapping.ProductId equals product.Id
                            join productCategory in ProductCategoryRepository.Table
                                on product.Id equals productCategory.ProductId into productCategories
                            from productCategory in productCategories.DefaultIfEmpty()
                            where (
                                      productCategory != null
                                      && categoryIds.Contains(productCategory.CategoryId)
                                      && (product.ParentGroupedProductId == 0 || product.VisibleIndividually)
                                  )
                                  || (
                                      productCategory == null
                                      && groupProductIds.Contains(product.ParentGroupedProductId)
                                  )
                            where (includeFeaturedProducts
                                   || productCategory == null
                                   || !productCategory.IsFeaturedProduct)
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            select productAttribute;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            }));
    }

    private async Task<IList<ProductAttribute>> GetAllProductAttributesByManufacturerIdInternalAsync(
        int manufacturerId,
        bool showHiddenProducts)
    {
        if (manufacturerId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTES_BY_MANUFACTURERID_KEY,
            manufacturerId,
            showHiddenProducts,
            store.Id);

        return new List<ProductAttribute>(await CacheManager.GetAsync<IEnumerable<ProductAttribute>>(
            cacheKey,
            async () =>
            {
                var groupProductIds = await _productServiceNopAjaxFilters
                    .GetAllGroupProductIdsInManufacturerAsync(manufacturerId);
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);
                var includeFeaturedProducts = await GetIncludeFeaturedProductsInNormalListAsync();

                var query = from productAttribute in ProductAttributeRepository.Table
                            join productAttributeMapping in ProductAttributeMappingRepository.Table
                                on productAttribute.Id equals productAttributeMapping.ProductAttributeId
                            join product in availableProducts
                                on productAttributeMapping.ProductId equals product.Id
                            join productManufacturer in _productManufacturerRepository.Table
                                on product.Id equals productManufacturer.ProductId into productManufacturers
                            from productManufacturer in productManufacturers.DefaultIfEmpty()
                            where (
                                      productManufacturer != null
                                      && productManufacturer.ManufacturerId == manufacturerId
                                      && (includeFeaturedProducts || !productManufacturer.IsFeaturedProduct)
                                      && (product.ParentGroupedProductId == 0 || product.VisibleIndividually)
                                  )
                                  || (
                                      productManufacturer == null
                                      && groupProductIds.Contains(product.ParentGroupedProductId)
                                  )
                            where (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            select productAttribute;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            }));
    }

    private async Task<IList<ProductAttribute>> GetAllProductAttributesByVendorIdInternalAsync(
        int vendorId,
        bool showHiddenProducts)
    {
        if (vendorId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTES_BY_VENDORID_KEY,
            vendorId,
            showHiddenProducts,
            store.Id);

        return new List<ProductAttribute>(await CacheManager.GetAsync<IEnumerable<ProductAttribute>>(
            cacheKey,
            async () =>
            {
                var groupProductIds = await _productServiceNopAjaxFilters
                    .GetAllGroupProductIdsInVendorAsync(vendorId);
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);

                var query = from productAttribute in ProductAttributeRepository.Table
                            join productAttributeMapping in ProductAttributeMappingRepository.Table
                                on productAttribute.Id equals productAttributeMapping.ProductAttributeId
                            join product in availableProducts
                                on productAttributeMapping.ProductId equals product.Id
                            join vendor in _vendorRepository
                                .Table on product.VendorId equals vendor.Id into vendors
                            from vendor in vendors.DefaultIfEmpty()
                            where (
                                      vendor != null
                                      && vendor.Id == vendorId
                                      && (product.ParentGroupedProductId == 0 || product.VisibleIndividually)
                                  )
                                  || (
                                      vendor == null
                                      && groupProductIds.Contains(product.ParentGroupedProductId)
                                  )
                            where (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            select productAttribute;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            }));
    }

    private async Task<IList<ProductAttribute>> GetAllProductAttributesByProductAttributeMappingIdsInternalAsync(
        IList<int> productAttributeMappingIds,
        bool showHiddenProducts)
    {
        if (productAttributeMappingIds == null || productAttributeMappingIds.Count == 0)
            return new List<ProductAttribute>();

        var mappingIds = string.Join(",", productAttributeMappingIds);
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTES_BY_PRODUCTVARIANTIDS_KEY,
            mappingIds,
            showHiddenProducts,
            store.Id);

        return new List<ProductAttribute>(await CacheManager.GetAsync<IEnumerable<ProductAttribute>>(
            cacheKey,
            async () =>
            {
                var query = from productAttribute in ProductAttributeRepository.Table
                            join productAttributeMapping in ProductAttributeMappingRepository.Table
                                on productAttribute.Id equals productAttributeMapping.ProductAttributeId
                            where productAttributeMappingIds.Contains(productAttributeMapping.Id)
                            select productAttribute;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            }));
    }

    private async Task<IList<ProductAttributeValue>> GetAllProductVariantAttributeValuesByProductAttributeIdAndCategoryIdInternalAsync(
        int productAttributeId,
        int categoryId,
        bool includeProductsInSubcategories,
        bool showHiddenProducts)
    {
        if (productAttributeId == 0 || categoryId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTEVALUES_BY_PRODUCTATTRIBUTEID_AND_CATEGORYID_KEY,
            productAttributeId,
            categoryId,
            includeProductsInSubcategories,
            showHiddenProducts,
            store.Id,
            language.Id);

        return await CacheManager.GetAsync<IList<ProductAttributeValue>>(
            cacheKey,
            async () =>
            {
                var categoryIds = new List<int> { categoryId };
                if (includeProductsInSubcategories)
                {
                    var childCategoryIds = await CategoryService7Spikes
                        .GetCategoryIdsByParentCategoryAsync(categoryId, false);
                    categoryIds.AddRange(childCategoryIds);
                }

                var groupProductIds = await _productServiceNopAjaxFilters
                    .GetAllGroupProductIdsInCategoriesAsync(categoryIds);
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);
                var includeFeaturedProducts = await GetIncludeFeaturedProductsInNormalListAsync();

                var query = from attributeValue in ProductAttributeValueRepository.Table
                            join attributeMapping in ProductAttributeMappingRepository.Table
                                on attributeValue.ProductAttributeMappingId equals attributeMapping.Id
                            join productAttribute in ProductAttributeRepository.Table
                                on attributeMapping.ProductAttributeId equals productAttribute.Id
                            join product in availableProducts
                                on attributeMapping.ProductId equals product.Id
                            join productCategory in ProductCategoryRepository.Table
                                    .Where(item => categoryIds.Contains(item.CategoryId))
                                on product.Id equals productCategory.ProductId into productCategories
                            from productCategory in productCategories.DefaultIfEmpty()
                            where productAttribute.Id == productAttributeId
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (
                                      (productCategory != null
                                       && (includeFeaturedProducts || !productCategory.IsFeaturedProduct)
                                       && (product.ParentGroupedProductId == 0 || product.VisibleIndividually))
                                      || (productCategory == null
                                          && groupProductIds.Contains(product.ParentGroupedProductId))
                                  )
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            orderby attributeValue.DisplayOrder
                            select attributeValue;

                return await LocalizeProductAttributeValuesAsync(query);
            });
    }

    private async Task<IList<ProductAttributeValue>> GetAllProductVariantAttributeValuesByProductAttributeIdAndManufacturerIdInternalAsync(
        int productAttributeId,
        int manufacturerId,
        bool showHiddenProducts)
    {
        if (productAttributeId == 0 || manufacturerId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTEVALUES_BY_PRODUCTATTRIBUTEID_AND_MANUFACTURERID_KEY,
            productAttributeId,
            manufacturerId,
            showHiddenProducts,
            store.Id,
            language.Id);

        return await CacheManager.GetAsync<IList<ProductAttributeValue>>(
            cacheKey,
            async () =>
            {
                var groupProductIds = await _productServiceNopAjaxFilters
                    .GetAllGroupProductIdsInManufacturerAsync(manufacturerId);
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);
                var includeFeaturedProducts = await GetIncludeFeaturedProductsInNormalListAsync();
                var manufacturerMappings = _productManufacturerRepository.Table
                    .Where(mapping => mapping.ManufacturerId == manufacturerId);

                var query = from attributeValue in ProductAttributeValueRepository.Table
                            join attributeMapping in ProductAttributeMappingRepository.Table
                                on attributeValue.ProductAttributeMappingId equals attributeMapping.Id
                            join productAttribute in ProductAttributeRepository.Table
                                on attributeMapping.ProductAttributeId equals productAttribute.Id
                            join product in availableProducts
                                on attributeMapping.ProductId equals product.Id
                            join manufacturerMapping in manufacturerMappings
                                on product.Id equals manufacturerMapping.ProductId into productManufacturers
                            from manufacturerMapping in productManufacturers.DefaultIfEmpty()
                            where productAttribute.Id == productAttributeId
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (
                                      (manufacturerMapping != null
                                       && (includeFeaturedProducts || !manufacturerMapping.IsFeaturedProduct)
                                       && (product.ParentGroupedProductId == 0 || product.VisibleIndividually))
                                      || (manufacturerMapping == null
                                          && groupProductIds.Contains(product.ParentGroupedProductId))
                                  )
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            orderby attributeValue.DisplayOrder
                            select attributeValue;

                return await LocalizeProductAttributeValuesAsync(query);
            });
    }

    private async Task<IList<ProductAttributeValue>> GetAllProductVariantAttributeValuesByProductAttributeIdAndVendorIdInternalAsync(
        int productAttributeId,
        int vendorId,
        bool showHiddenProducts)
    {
        if (productAttributeId == 0 || vendorId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTEVALUES_BY_PRODUCTATTRIBUTEID_AND_VENDORID_KEY,
            productAttributeId,
            vendorId,
            showHiddenProducts,
            store.Id,
            language.Id);

        return await CacheManager.GetAsync<IList<ProductAttributeValue>>(
            cacheKey,
            async () =>
            {
                var groupProductIds = await _productServiceNopAjaxFilters
                    .GetAllGroupProductIdsInVendorAsync(vendorId);
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);
                var vendors = _vendorRepository.Table.Where(vendor => vendor.Id == vendorId);

                var query = from attributeValue in ProductAttributeValueRepository.Table
                            join attributeMapping in ProductAttributeMappingRepository.Table
                                on attributeValue.ProductAttributeMappingId equals attributeMapping.Id
                            join productAttribute in ProductAttributeRepository.Table
                                on attributeMapping.ProductAttributeId equals productAttribute.Id
                            join product in availableProducts
                                on attributeMapping.ProductId equals product.Id
                            join vendor in vendors
                                on product.VendorId equals vendor.Id into productVendors
                            from vendor in productVendors.DefaultIfEmpty()
                            where productAttribute.Id == productAttributeId
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (
                                      (vendor != null
                                       && (product.ParentGroupedProductId == 0 || product.VisibleIndividually))
                                      || (vendor == null
                                          && groupProductIds.Contains(product.ParentGroupedProductId))
                                  )
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            orderby attributeValue.DisplayOrder
                            select attributeValue;

                return await LocalizeProductAttributeValuesAsync(query);
            });
    }

    private async Task<IList<ProductAttributeValue>> LocalizeProductAttributeValuesAsync(
        IQueryable<ProductAttributeValue> attributeValuesQuery)
    {
        var languages = await _languageService.GetAllLanguagesAsync(false, 0);
        if (languages.Count < 2)
            return await AsyncIQueryableExtensions.ToListAsync(attributeValuesQuery);

        var workingLanguage = await _workContext.GetWorkingLanguageAsync();
        var localizedProperties = LocalizedPropertyRepository.Table.Where(property =>
            property.LocaleKeyGroup == "ProductAttributeValue"
            && property.LocaleKey == "Name"
            && property.LanguageId == workingLanguage.Id);

        var query = from attributeValue in attributeValuesQuery
                    join localizedProperty in localizedProperties
                        on attributeValue.Id equals localizedProperty.EntityId into localizedValues
                    from localizedProperty in localizedValues.DefaultIfEmpty()
                    orderby attributeValue.DisplayOrder
                    select new
                    {
                        AttributeValue = attributeValue,
                        LocalizedName = localizedProperty.LocaleValue
                    };

        var results = await AsyncIQueryableExtensions.ToListAsync(query);
        foreach (var result in results)
        {
            if (!string.IsNullOrEmpty(result.LocalizedName))
                result.AttributeValue.Name = result.LocalizedName;
        }

        return results.Select(result => result.AttributeValue).ToList();
    }

    private async Task<IList<ProductAttributeMapping>> GetAllProductVariantAttributesByProductIdInternalAsync(
        int productId,
        bool showHiddenProducts)
    {
        if (productId == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTVARIANTATTRIBUTES_BY_PRODUCTID_KEY,
            productId,
            showHiddenProducts,
            store.Id);

        return new List<ProductAttributeMapping>(await CacheManager.GetAsync<IEnumerable<ProductAttributeMapping>>(
            cacheKey,
            async () =>
            {
                var nowUtc = DateTime.UtcNow;
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                var query = from attributeMapping in ProductAttributeMappingRepository.Table
                            join productAttribute in ProductAttributeRepository.Table
                                on attributeMapping.ProductAttributeId equals productAttribute.Id
                            join product in availableProducts
                                on attributeMapping.ProductId equals product.Id
                            where product.Id == productId
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            select attributeMapping;

                return await AsyncIQueryableExtensions.ToListAsync(query);
            }));
    }

    private async Task<IList<ProductAttributeMapping>> GetAllProductVariantAttributesWhichHaveValuesByProductIdInternalAsync(
        int productId,
        bool showHiddenProducts)
    {
        if (productId == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTVARIANTATTRIBUTES_WHICH_HAVE_VALUES_BY_PRODUCTID_KEY,
            productId,
            showHiddenProducts,
            store.Id);

        return new List<ProductAttributeMapping>(await CacheManager.GetAsync<IEnumerable<ProductAttributeMapping>>(
            cacheKey,
            async () =>
            {
                var nowUtc = DateTime.UtcNow;
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                var query = from attributeMapping in ProductAttributeMappingRepository.Table
                            join productAttribute in ProductAttributeRepository.Table
                                on attributeMapping.ProductAttributeId equals productAttribute.Id
                            join attributeValue in ProductAttributeValueRepository.Table
                                on attributeMapping.Id equals attributeValue.ProductAttributeMappingId
                            join product in availableProducts
                                on attributeMapping.ProductId equals product.Id
                            where product.Id == productId
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            select attributeMapping;

                return await AsyncIQueryableExtensions.ToListAsync(query);
            }));
    }

    private async Task<IList<ProductAttributeMapping>> GetAllProductVariantAttributesWhichHaveValuesByProductIdsInternalAsync(
        IList<int> productIds,
        bool showHiddenProducts)
    {
        if (productIds == null || productIds.Count == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTVARIANTATTRIBUTES_WHICH_HAVE_VALUES_BY_PRODUCTIDS_KEY,
            productIds,
            showHiddenProducts,
            store.Id);

        return await CacheManager.GetAsync<IList<ProductAttributeMapping>>(
            cacheKey,
            async () =>
            {
                var nowUtc = DateTime.UtcNow;
                var ids = productIds.ToList();
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                var query = from attributeMapping in ProductAttributeMappingRepository.Table
                            join productAttribute in ProductAttributeRepository.Table
                                on attributeMapping.ProductAttributeId equals productAttribute.Id
                            join attributeValue in ProductAttributeValueRepository.Table
                                on attributeMapping.Id equals attributeValue.ProductAttributeMappingId
                            join product in availableProducts
                                on attributeMapping.ProductId equals product.Id
                            where ids.Contains(product.Id)
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                            select attributeMapping;

                return await AsyncIQueryableExtensions.ToListAsync(query);
            });
    }

    private async Task<IList<ProductAttributeProductAttributeValueDTO>> GetProductAttributeProductAttributeValueDtosByProductAttributeMappingIdsInternalAsync(
        IList<int> productAttributeMappingIds)
    {
        if (productAttributeMappingIds == null || productAttributeMappingIds.Count == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();
        var cacheKey = CacheManager.PrepareKeyForDefaultCache(
            PRODUCTATTRIBUTEVALUES_AND_PRODUCT_ATTRIBUTES_BY_PRODUCTATTRIBUTEMAPPINGS_KEY,
            productAttributeMappingIds,
            store.Id,
            language.Id);

        return await CacheManager.GetAsync<IList<ProductAttributeProductAttributeValueDTO>>(
            cacheKey,
            async () =>
            {
                var query = from attributeValue in ProductAttributeValueRepository.Table
                            join attributeMapping in ProductAttributeMappingRepository.Table
                                on attributeValue.ProductAttributeMappingId equals attributeMapping.Id
                            where productAttributeMappingIds.Contains(attributeMapping.Id)
                            orderby attributeValue.DisplayOrder
                            select new ProductAttributeProductAttributeValueDTO
                            {
                                ProductAttributeValue = attributeValue
                            };

                return await AsyncIQueryableExtensions.ToListAsync(query);
            });
    }

    private async Task<bool> GetIncludeFeaturedProductsInNormalListAsync()
    {
        return await SettingService.GetSettingByKeyAsync<bool>("catalogsettings.includefeaturedproductsinnormallists", false, 0, false);
    }
}
