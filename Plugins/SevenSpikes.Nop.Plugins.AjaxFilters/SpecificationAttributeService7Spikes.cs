using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Helpers;

public class SpecificationAttributeService7Spikes : ISpecificationAttributeService7Spikes
{
    private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
    private readonly IAclHelper _aclHelper;
    private readonly IStoreContext _storeContext;
    private readonly IStoreHelper _storeHelper;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
    private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
    private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
    private readonly ICategoryService7Spikes _categoryService;
    private readonly ISettingService _settingService;
    private readonly IStaticCacheManager _cacheManager;

    private CacheKey SpecificationAttributeOptionsDictionaryCacheKey =>
        new("Nop.specificationattributeoptions.dictionary.store.id-{0}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsByCategoryCacheKey =>
        new("Nop.specificationattributeoptions.categoryid-{0}.includeproductsinsubcategories-{1}.showhidden-{2}.store.id-{3}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsByManufacturerCacheKey =>
        new("Nop.specificationattributeoptions.manufacturerid-{0}.showhidden-{1}.store.id-{2}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsByVendorCacheKey =>
        new("Nop.specificationattributeoptions.vendorid-{0}.showhidden-{1}.store.id-{2}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsByIdsCacheKey =>
        new("Nop.specificationattributeoptions.ids-{0}.store.id-{1}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsByProductIdCacheKey =>
        new("Nop.specificationattributeoptions.productid-{0}.includenotallowfiltering-{1}.store.id-{2}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsByProductIdsCacheKey =>
        new("Nop.specificationattributeoptions.productids-{0}.showhidden-{1}.store.id-{2}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsAllCacheKey =>
        new("Nop.specificationattributeoptions.all.includenotallowfiltering-{0}.store.id-{1}", Array.Empty<string>());

    private CacheKey SpecificationAttributeOptionsByIdsAndSpecificationIdCacheKey =>
        new("Nop.specificationattributeoptions.ids-{0}.specification.id-{1}.store.id-{2}", Array.Empty<string>());

    public SpecificationAttributeService7Spikes(
        IRepository<ProductManufacturer> productManufacturerRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
        IRepository<SpecificationAttribute> specificationAttributeRepository,
        IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
        ISettingService settingService,
        IStaticCacheManager cacheManager,
        IAclHelper aclHelper,
        IStoreContext storeContext,
        IStoreHelper storeHelper,
        ICategoryService7Spikes categoryService)
    {
        _productManufacturerRepository = productManufacturerRepository;
        _productCategoryRepository = productCategoryRepository;
        _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
        _specificationAttributeRepository = specificationAttributeRepository;
        _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
        _settingService = settingService;
        _cacheManager = cacheManager;
        _aclHelper = aclHelper;
        _storeContext = storeContext;
        _storeHelper = storeHelper;
        _categoryService = categoryService;
    }

    public virtual Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsAndSpecificationIdAsync(
        IList<int> specificationAttributeOptionIds,
        int specificationId)
    {
        return GetSpecificationAttributeOptionsByIdsAndSpecificationIdInternalAsync(
            specificationAttributeOptionIds,
            specificationId);
    }

    public virtual Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByCategoryIdAsync(
        int categoryId,
        bool includeProductsInSubcategories = false,
        bool showHiddenProducts = false)
    {
        return GetSpecificationAttributeOptionsByCategoryIdInternalAsync(
            categoryId,
            includeProductsInSubcategories,
            showHiddenProducts);
    }

    public Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByManufacturerIdAsync(
        int manufacturerId,
        bool showHiddenProducts = false)
    {
        return GetSpecificationAttributeOptionsByManufacturerIdInternalAsync(
            manufacturerId,
            showHiddenProducts);
    }

    public Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByVendorIdAsync(
        int vendorId,
        bool showHiddenProducts = false)
    {
        return GetSpecificationAttributeOptionsByVendorIdInternalAsync(vendorId, showHiddenProducts);
    }

    public virtual Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsAsync(
        IList<int> specificationAttributeOptionIds)
    {
        return GetSpecificationAttributeOptionsByIdsInternalAsync(specificationAttributeOptionIds);
    }

    public virtual Task<IList<SpecificationAttributeOption>> GetAllSpecificationAttributeOptionsAsync(
        bool includeNotAllowFilteringOptions = false)
    {
        return GetAllSpecificationAttributeOptionsInternalAsync(includeNotAllowFilteringOptions);
    }

    public virtual Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByProductIdAsync(
        int productId,
        bool includeNotAllowFilteringOptions = false)
    {
        return GetSpecificationAttributeOptionsByProductIdInternalAsync(
            productId,
            includeNotAllowFilteringOptions);
    }

    public virtual Task<IEnumerable<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByProductIdsAsync(
        IList<int> productIds,
        bool showHiddenProducts = false)
    {
        return GetSpecificationAttributeOptionsByProductIdsInternalAsync(productIds, showHiddenProducts);
    }

    public virtual Task<IDictionary<int, IList<int>>> GetSpecificationAttributeOptionsDictionaryForProductsAsync(
        IQueryable<Product> products)
    {
        return GetSpecificationAttributeOptionsDictionaryForProductsInternalAsync(products);
    }

    public virtual Task<IDictionary<int, IList<int>>> GetSpecificationAttributeOptionsDictionaryAsync()
    {
        return GetSpecificationAttributeOptionsDictionaryInternalAsync();
    }

    private async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsAndSpecificationIdInternalAsync(
        IList<int> specificationAttributeOptionIds,
        int specificationId)
    {
        if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Count == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsByIdsAndSpecificationIdCacheKey,
            specificationAttributeOptionIds,
            specificationId,
            store.Id);

        return await _cacheManager.GetAsync<IList<SpecificationAttributeOption>>(
            cacheKey,
            async () => await AsyncIQueryableExtensions.ToListAsync(
                _specificationAttributeOptionRepository.Table.Where(option =>
                    specificationAttributeOptionIds.Contains(option.Id)
                    && option.SpecificationAttributeId == specificationId)));
    }

    private async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByCategoryIdInternalAsync(
        int categoryId,
        bool includeProductsInSubcategories,
        bool showHiddenProducts)
    {
        if (categoryId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsByCategoryCacheKey,
            categoryId,
            includeProductsInSubcategories,
            showHiddenProducts,
            store.Id);

        return await _cacheManager.GetAsync<IList<SpecificationAttributeOption>>(
            cacheKey,
            async () =>
            {
                var subCategoryIds = includeProductsInSubcategories
                    ? await _categoryService.GetCategoryIdsByParentCategoryAsync(categoryId, false)
                    : new List<int>();

                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);
                var includeFeaturedProducts = await GetIncludeFeaturedProductsInNormalListAsync();

                var query = from option in _specificationAttributeOptionRepository.Table
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            join specification in _specificationAttributeRepository.Table
                                on option.SpecificationAttributeId equals specification.Id
                            join product in availableProducts
                                on productSpecification.ProductId equals product.Id
                            join productCategory in _productCategoryRepository.Table
                                on product.Id equals productCategory.ProductId
                            where (productCategory.CategoryId == categoryId
                                   || (includeProductsInSubcategories
                                       && subCategoryIds.Contains(productCategory.CategoryId)))
                                  && (includeFeaturedProducts || !productCategory.IsFeaturedProduct)
                                  && (product.ParentGroupedProductId == 0 || product.VisibleIndividually)
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                                  && productSpecification.AllowFiltering
                            select option;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            });
    }

    private async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByManufacturerIdInternalAsync(
        int manufacturerId,
        bool showHiddenProducts)
    {
        if (manufacturerId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsByManufacturerCacheKey,
            manufacturerId,
            showHiddenProducts,
            store.Id);

        return await _cacheManager.GetAsync<IList<SpecificationAttributeOption>>(
            cacheKey,
            async () =>
            {
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);
                var includeFeaturedProducts = await GetIncludeFeaturedProductsInNormalListAsync();

                var query = from option in _specificationAttributeOptionRepository.Table
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            join specification in _specificationAttributeRepository.Table
                                on option.SpecificationAttributeId equals specification.Id
                            join product in availableProducts
                                on productSpecification.ProductId equals product.Id
                            join productManufacturer in _productManufacturerRepository.Table
                                on product.Id equals productManufacturer.ProductId
                            where productManufacturer.ManufacturerId == manufacturerId
                                  && (includeFeaturedProducts || !productManufacturer.IsFeaturedProduct)
                                  && (product.ParentGroupedProductId == 0 || product.VisibleIndividually)
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                                  && productSpecification.AllowFiltering
                            select option;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            });
    }

    private async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByVendorIdInternalAsync(
        int vendorId,
        bool showHiddenProducts)
    {
        if (vendorId == 0)
            return null;

        var nowUtc = DateTime.UtcNow;
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsByVendorCacheKey,
            vendorId,
            showHiddenProducts,
            store.Id);

        return await _cacheManager.GetAsync<IList<SpecificationAttributeOption>>(
            cacheKey,
            async () =>
            {
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                availableProducts = await _storeHelper.GetProductsForCurrentStoreAsync(availableProducts);

                var query = from option in _specificationAttributeOptionRepository.Table
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            join product in availableProducts
                                on productSpecification.ProductId equals product.Id
                            where product.VendorId == vendorId
                                  && (product.ParentGroupedProductId == 0 || product.VisibleIndividually)
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                                  && productSpecification.AllowFiltering
                            select option;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            });
    }

    private async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByIdsInternalAsync(
        IList<int> specificationAttributeOptionIds)
    {
        if (specificationAttributeOptionIds == null || specificationAttributeOptionIds.Count == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsByIdsCacheKey,
            specificationAttributeOptionIds,
            store.Id);

        return await _cacheManager.GetAsync<IList<SpecificationAttributeOption>>(
            cacheKey,
            async () =>
            {
                var ids = specificationAttributeOptionIds.ToList();
                var query = from option in _specificationAttributeOptionRepository.Table
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            where productSpecification.AllowFiltering && ids.Contains(option.Id)
                            select option;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            });
    }

    private async Task<IList<SpecificationAttributeOption>> GetAllSpecificationAttributeOptionsInternalAsync(
        bool includeNotAllowFilteringOptions)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsAllCacheKey,
            includeNotAllowFilteringOptions,
            store.Id);

        return await _cacheManager.GetAsync<IList<SpecificationAttributeOption>>(
            cacheKey,
            async () =>
            {
                var query = from option in _specificationAttributeOptionRepository.Table
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            where includeNotAllowFilteringOptions || productSpecification.AllowFiltering
                            select option;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            });
    }

    private async Task<IList<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByProductIdInternalAsync(
        int productId,
        bool includeNotAllowFilteringOptions)
    {
        if (productId == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsByProductIdCacheKey,
            productId,
            includeNotAllowFilteringOptions,
            store.Id);

        return await _cacheManager.GetAsync<IList<SpecificationAttributeOption>>(
            cacheKey,
            async () =>
            {
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
                var query = from option in _specificationAttributeOptionRepository.Table
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            join product in availableProducts
                                on productSpecification.ProductId equals product.Id
                            where (includeNotAllowFilteringOptions || productSpecification.AllowFiltering)
                                  && product.Id == productId
                            select option;

                return await AsyncIQueryableExtensions.ToListAsync(query);
            });
    }

    private async Task<IEnumerable<SpecificationAttributeOption>> GetSpecificationAttributeOptionsByProductIdsInternalAsync(
        IList<int> productIds,
        bool showHiddenProducts)
    {
        if (productIds == null || productIds.Count == 0)
            return null;

        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsByProductIdsCacheKey,
            productIds,
            showHiddenProducts,
            store.Id);

        return await _cacheManager.GetAsync<IEnumerable<SpecificationAttributeOption>>(
            cacheKey,
            async () =>
            {
                var nowUtc = DateTime.UtcNow;
                var ids = productIds.ToList();
                var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();

                var query = from option in _specificationAttributeOptionRepository.Table
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            join product in availableProducts
                                on productSpecification.ProductId equals product.Id
                            where ids.Contains(product.Id)
                                  && (showHiddenProducts || product.Published)
                                  && !product.Deleted
                                  && (!product.AvailableStartDateTimeUtc.HasValue
                                      || product.AvailableStartDateTimeUtc <= nowUtc)
                                  && (!product.AvailableEndDateTimeUtc.HasValue
                                      || product.AvailableEndDateTimeUtc >= nowUtc)
                                  && productSpecification.AllowFiltering
                            orderby option.DisplayOrder
                            select option;

                return await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
            });
    }

    private async Task<IDictionary<int, IList<int>>> GetSpecificationAttributeOptionsDictionaryForProductsInternalAsync(
        IQueryable<Product> products)
    {
        if (products == null)
            return null;

        // Read candidate IDs once, asynchronously. The previous Any() + ToList() sequence
        // executed two synchronous queries before loading the specification mappings.
        var productIds = await AsyncIQueryableExtensions.ToListAsync(
            products.Select(product => product.Id).Distinct());

        if (productIds.Count == 0)
            return null;

        var availableProducts = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
        var result = new Dictionary<int, IList<int>>();

        // Keep IN lists bounded for SQL Server and other providers with parameter limits.
        foreach (var productIdBatch in productIds.Chunk(1000))
        {
            var ids = productIdBatch.ToList();
            var query = from option in _specificationAttributeOptionRepository.Table
                        join productSpecification in _productSpecificationAttributeRepository.Table
                            on option.Id equals productSpecification.SpecificationAttributeOptionId
                        join product in availableProducts
                            on productSpecification.ProductId equals product.Id
                        where ids.Contains(product.Id) && productSpecification.AllowFiltering
                        select new
                        {
                            ProductId = product.Id,
                            SpecificationAttributeOptionId = option.Id
                        };

            var rows = await AsyncIQueryableExtensions.ToListAsync(
                query.OrderBy(row => row.ProductId));

            foreach (var group in rows.GroupBy(row => row.ProductId))
                result[group.Key] = group.Select(row => row.SpecificationAttributeOptionId).ToList();
        }

        return result;
    }

    private async Task<IDictionary<int, IList<int>>> GetSpecificationAttributeOptionsDictionaryInternalAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(
            SpecificationAttributeOptionsDictionaryCacheKey,
            store.Id);

        var rows = await _cacheManager.GetAsync<List<Tuple<int, int>>>(
            cacheKey,
            async () =>
            {
                var query = from option in _specificationAttributeOptionRepository.Table
                            join specification in _specificationAttributeRepository.Table
                                on option.SpecificationAttributeId equals specification.Id
                            join productSpecification in _productSpecificationAttributeRepository.Table
                                on option.Id equals productSpecification.SpecificationAttributeOptionId
                            where productSpecification.AllowFiltering
                            orderby specification.Id
                            select new
                            {
                                SpecificationAttributeId = specification.Id,
                                SpecificationAttributeOptionId = option.Id
                            };

                var options = await AsyncIQueryableExtensions.ToListAsync(query.Distinct());
                return options
                    .Select(row => Tuple.Create(row.SpecificationAttributeId, row.SpecificationAttributeOptionId))
                    .ToList();
            });

        var result = new Dictionary<int, IList<int>>();
        foreach (var group in rows.GroupBy(row => row.Item1))
        {
            var optionIds = group.Select(row => row.Item2).ToList();
            foreach (var optionId in optionIds)
                result[optionId] = optionIds;
        }

        return result;
    }

    private Task<bool> GetIncludeFeaturedProductsInNormalListAsync()
    {
        return _settingService.GetSettingByKeyAsync<bool>(
            "catalogsettings.includefeaturedproductsinnormallists",
            false,
            0,
            false);
    }
}
