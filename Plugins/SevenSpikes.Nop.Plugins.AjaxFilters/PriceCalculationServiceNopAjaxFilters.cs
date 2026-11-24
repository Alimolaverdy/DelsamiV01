using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Directory;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.PriceRangeFilterSlider;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Helpers;

public class PriceCalculationServiceNopAjaxFilters : IPriceCalculationServiceNopAjaxFilters
{
    private readonly IWorkContext _workContext;

    private readonly CatalogSettings _catalogSettings;

    private readonly TaxSettings _taxSettings;

    private readonly ICategoryService7Spikes _categoryService7Spikes;

    private readonly IAclHelper _aclHelper;

    private readonly IRepository<ProductCategory> _productCategoryRepository;

    private readonly IRepository<ProductManufacturer> _productManufacturerRepository;

    private readonly IRepository<Vendor> _vendorRepository;

    private readonly ITaxServiceNopAjaxFilters _taxServiceNopAjaxFilters;

    private readonly ICurrencyService _currencyService;

    private readonly IProductServiceNopAjaxFilters _productServiceNopAjaxFilters;

    private readonly IStoreHelper _storeHelper;

    private readonly ICustomerService _customerService;

    public PriceCalculationServiceNopAjaxFilters(IWorkContext workContext, CatalogSettings catalogSettings, TaxSettings taxSettings, ICategoryService7Spikes categoryService7Spikes, IAclHelper aclHelper, IRepository<ProductCategory> productCategoryRepository, IRepository<ProductManufacturer> productManufacturerRepository, IRepository<Vendor> vendorRepository, ITaxServiceNopAjaxFilters taxServiceNopAjaxFilters, ICurrencyService currencyService, IProductServiceNopAjaxFilters productServiceNopAjaxFilters, IStoreHelper storeHelper, ICustomerService customerService)
    {
        _workContext = workContext;
        _catalogSettings = catalogSettings;
        _taxSettings = taxSettings;
        _categoryService7Spikes = categoryService7Spikes;
        _aclHelper = aclHelper;
        _vendorRepository = vendorRepository;
        _productCategoryRepository = productCategoryRepository;
        _productManufacturerRepository = productManufacturerRepository;
        _taxServiceNopAjaxFilters = taxServiceNopAjaxFilters;
        _currencyService = currencyService;
        _productServiceNopAjaxFilters = productServiceNopAjaxFilters;
        _storeHelper = storeHelper;
        _customerService = customerService;
    }

    public async Task<PriceRangeFilterDto> GetPriceRangeFilterDtoAsync(
        int categoryId,
        int manufacturerId,
        int vendorId)
    {
        var model = new PriceRangeFilterDto();
        if (categoryId > 0)
            SetDiscountAmountPercentageForCategory(categoryId, model);

        var taxDisplayType = await _workContext.GetTaxDisplayTypeAsync();
        if ((int)taxDisplayType == 10)
            model.TaxDisplayTypeIncludingTax = false;
        else if ((int)taxDisplayType == 0)
            model.TaxDisplayTypeIncludingTax = true;

        model.TaxPriceIncludeTax = _taxSettings.PricesIncludeTax;
        await SetMinMaxPricesAsync(categoryId, manufacturerId, vendorId, model);

        var workingCurrency = await _workContext.GetWorkingCurrencyAsync();
        model.MinPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(
            model.MinPrice,
            workingCurrency);
        model.MaxPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(
            model.MaxPrice,
            workingCurrency);

        return model;
    }

    public async Task<decimal> CalculateBasePriceAsync(
        decimal price,
        PriceRangeFilterDto priceRangeModel,
        bool isFromPrice)
    {
        price = GetPriceWithoutDiscount(price, priceRangeModel);
        price = GetPriceWithoutTax(price, priceRangeModel);

        var workingCurrency = await _workContext.GetWorkingCurrencyAsync();
        price = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(price, workingCurrency);

        return Math.Round(price, 2);
    }

    private static decimal GetPriceWithoutTax(decimal price, PriceRangeFilterDto priceRangeModel)
    {
        if (priceRangeModel.TaxPriceIncludeTax && !priceRangeModel.TaxDisplayTypeIncludingTax)
            return CalculatePriceWithoutTax(price, priceRangeModel.TaxRatePercentage, increase: false);

        if (!priceRangeModel.TaxPriceIncludeTax && priceRangeModel.TaxDisplayTypeIncludingTax)
            return CalculatePriceWithoutTax(price, priceRangeModel.TaxRatePercentage, increase: true);

        return price;
    }

    private static decimal CalculatePriceWithoutTax(decimal price, decimal percent, bool increase)
    {
        if (percent == 0m)
        {
            return price;
        }
        if (increase)
        {
            return price / (1m + percent / 100m);
        }
        return price * (100m + percent) / (100m + percent - percent);
    }

    private static decimal GetPriceWithoutDiscount(decimal price, PriceRangeFilterDto priceRangeModel)
    {
        if (priceRangeModel.MaxDiscountAmount == 0m && priceRangeModel.MaxDiscountPercentage == 0m)
        {
            return price;
        }
        decimal result = price;
        decimal num = (decimal)((float)price * 100f / (100f - (float)priceRangeModel.MaxDiscountPercentage));
        decimal num2 = price + priceRangeModel.MaxDiscountAmount;
        if (num > 0m && num > num2)
        {
            result = num;
        }
        else if (num2 > 0m)
        {
            result = num2;
        }
        return result;
    }

    private async Task SetMinMaxPricesAsync(int categoryId, int manufacturerId, int vendorId, PriceRangeFilterDto priceRangeModel)
    {
        var source = await PrepareMinMaxPriceProductVariantQueryAsync(categoryId, manufacturerId, vendorId);

        // Aggregate min/max in one asynchronous database query instead of separate synchronous
        // Any/Min/Max calls, then load one representative product for its tax category.
        var priceRanges = await AsyncIQueryableExtensions.ToListAsync(
            source.GroupBy(product => 1)
                .Select(products => new
                {
                    MinPrice = products.Min(product => (decimal?)product.Price),
                    MaxPrice = products.Max(product => (decimal?)product.Price)
                }));

        var priceRange = priceRanges.FirstOrDefault();
        if (priceRange == null)
            return;

        var productVariant = await AsyncIQueryableExtensions.FirstOrDefaultAsync<Product>(
            source.Take(1),
            (Expression<Func<Product, bool>>)null);

        if (productVariant == null)
            return;

        SetMinPrice(priceRangeModel, priceRange.MinPrice);
        SetMaxPrice(priceRangeModel, priceRange.MaxPrice);
        priceRangeModel.TaxRatePercentage = await GetTaxRatePercentageAsync(productVariant);

        if (priceRangeModel.TaxRatePercentage > 0m)
            SetTaxForMinMaxPrice(priceRangeModel);
    }

    private void SetTaxForMinMaxPrice(PriceRangeFilterDto priceRangeModel)
    {
        if (priceRangeModel.TaxPriceIncludeTax)
        {
            if (!priceRangeModel.TaxDisplayTypeIncludingTax)
            {
                priceRangeModel.MinPrice = CalculatePrice(priceRangeModel.MinPrice, priceRangeModel.TaxRatePercentage, increase: false);
                priceRangeModel.MaxPrice = CalculatePrice(priceRangeModel.MaxPrice, priceRangeModel.TaxRatePercentage, increase: false);
            }
        }
        else if (priceRangeModel.TaxDisplayTypeIncludingTax)
        {
            priceRangeModel.MinPrice = CalculatePrice(priceRangeModel.MinPrice, priceRangeModel.TaxRatePercentage, increase: true);
            priceRangeModel.MaxPrice = CalculatePrice(priceRangeModel.MaxPrice, priceRangeModel.TaxRatePercentage, increase: true);
        }
    }

    private decimal CalculatePrice(decimal? nullablePrice, decimal percent, bool increase)
    {
        var price = nullablePrice.GetValueOrDefault();
        if (percent == 0m)
            return price;

        if (increase)
            return price * (1m + percent / 100m);

        return price - price / (100m + percent) * percent;
    }

    private async Task<decimal> GetTaxRatePercentageAsync(Product productVariant)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (customer != null
            && (customer.IsTaxExempt
                || (await _customerService.GetCustomerRolesAsync(customer, false)).Any(role => role.TaxExempt)))
        {
            return 0m;
        }

        return await _taxServiceNopAjaxFilters.GetTaxRateForProductAsync(productVariant, 0, customer);
    }

    private void SetMaxPrice(PriceRangeFilterDto priceRangeModel, decimal? maxPrice)
    {
        if (!maxPrice.HasValue)
        {
            priceRangeModel.MaxPrice = 0m;
            return;
        }
        maxPrice = ApplyDiscount(maxPrice.Value, priceRangeModel);
        priceRangeModel.MaxPrice = maxPrice.Value;
    }

    private void SetMinPrice(PriceRangeFilterDto priceRangeModel, decimal? minPrice)
    {
        if (!minPrice.HasValue)
        {
            priceRangeModel.MinPrice = 0m;
            return;
        }
        minPrice = ApplyDiscount(minPrice.Value, priceRangeModel);
        priceRangeModel.MinPrice = minPrice.Value;
    }

    private decimal ApplyDiscount(decimal price, PriceRangeFilterDto priceRangeModel)
    {
        decimal result = default(decimal);
        decimal num = price - (decimal)((float)price * (float)priceRangeModel.MaxDiscountPercentage / 100f);
        decimal num2 = price - priceRangeModel.MaxDiscountAmount;
        if (num > 0m && num < num2)
        {
            return num;
        }
        if (num2 > 0m)
        {
            return num2;
        }
        return result;
    }

    private async Task<IQueryable<Product>> PrepareMinMaxPriceProductVariantQueryAsync(int categoryId, int manufacturerId, int vendorId)
    {
        DateTime nowUtc = DateTime.UtcNow;
        IQueryable<Product> productsQuery = await _aclHelper.GetAvailableProductsForCurrentCustomerAsync();
        productsQuery = await _storeHelper.GetProductsForCurrentStoreAsync(productsQuery);
        if (manufacturerId > 0)
        {
            return await PrepareMinMaxPriceProductVariantForManufacturerQueryAsync(manufacturerId, productsQuery, nowUtc);
        }
        if (vendorId > 0)
        {
            return await PrepareMinMaxPriceProductVariantForVendorQueryAsync(vendorId, productsQuery, nowUtc);
        }
        return await PrepareMinMaxPriceProductVariantForCategoryQueryAsync(categoryId, productsQuery, nowUtc);
    }

    private async Task<IQueryable<Product>> PrepareMinMaxPriceProductVariantForManufacturerQueryAsync(
        int manufacturerId,
        IQueryable<Product> availableProductsQuery,
        DateTime nowUtc)
    {
        var includeFeaturedProducts = _catalogSettings.IncludeFeaturedProductsInNormalLists;
        var groupProductIds = await _productServiceNopAjaxFilters
            .GetAllGroupProductIdsInManufacturerAsync(manufacturerId);

        return from product in availableProductsQuery
               join productManufacturer in _productManufacturerRepository.Table
                   on product.Id equals productManufacturer.ProductId into productManufacturers
               from productManufacturer in productManufacturers.DefaultIfEmpty()
               where (
                         productManufacturer != null
                         && productManufacturer.ManufacturerId == manufacturerId
                         && (productManufacturer.IsFeaturedProduct == includeFeaturedProducts
                             || !productManufacturer.IsFeaturedProduct)
                     )
                     || (
                         productManufacturer == null
                         && groupProductIds.Contains(product.ParentGroupedProductId)
                     )
               where product.Published
                     && !product.Deleted
                     && (!product.AvailableStartDateTimeUtc.HasValue
                         || product.AvailableStartDateTimeUtc <= nowUtc)
                     && (!product.AvailableEndDateTimeUtc.HasValue
                         || product.AvailableEndDateTimeUtc >= nowUtc)
               select product;
    }

    private async Task<IQueryable<Product>> PrepareMinMaxPriceProductVariantForVendorQueryAsync(
        int vendorId,
        IQueryable<Product> availableProductsQuery,
        DateTime nowUtc)
    {
        var groupProductIds = await _productServiceNopAjaxFilters
            .GetAllGroupProductIdsInVendorAsync(vendorId);

        return from product in availableProductsQuery
               join vendor in _vendorRepository.Table
                   on product.VendorId equals vendor.Id into vendors
               from vendor in vendors.DefaultIfEmpty()
               where (vendor != null && vendor.Id == vendorId)
                     || (vendor == null
                         && groupProductIds.Contains(product.ParentGroupedProductId))
               where product.Published
                     && !product.Deleted
                     && (!product.AvailableStartDateTimeUtc.HasValue
                         || product.AvailableStartDateTimeUtc <= nowUtc)
                     && (!product.AvailableEndDateTimeUtc.HasValue
                         || product.AvailableEndDateTimeUtc >= nowUtc)
               select product;
    }

    private async Task<IQueryable<Product>> PrepareMinMaxPriceProductVariantForCategoryQueryAsync(
        int categoryId,
        IQueryable<Product> availableProductsQuery,
        DateTime nowUtc)
    {
        var showProductsFromSubcategories = _catalogSettings.ShowProductsFromSubcategories;
        var includeFeaturedProducts = _catalogSettings.IncludeFeaturedProductsInNormalLists;
        var categoryIds = new List<int> { categoryId };

        if (showProductsFromSubcategories)
        {
            var childCategoryIds = await _categoryService7Spikes
                .GetCategoryIdsByParentCategoryAsync(categoryId, false);
            categoryIds.AddRange(childCategoryIds);
        }

        var groupProductIds = await _productServiceNopAjaxFilters
            .GetAllGroupProductIdsInCategoriesAsync(categoryIds);

        return from product in availableProductsQuery
               join productCategory in _productCategoryRepository.Table
                   on product.Id equals productCategory.ProductId into productCategories
               from productCategory in productCategories.DefaultIfEmpty()
               where (
                         productCategory != null
                         && categoryIds.Contains(productCategory.CategoryId)
                         && product.ProductTypeId != 10
                         && (productCategory.IsFeaturedProduct == includeFeaturedProducts
                             || !productCategory.IsFeaturedProduct)
                     )
                     || (
                         productCategory == null
                         && groupProductIds.Contains(product.ParentGroupedProductId)
                     )
               where product.Published
                     && !product.Deleted
                     && (!product.AvailableStartDateTimeUtc.HasValue
                         || product.AvailableStartDateTimeUtc <= nowUtc)
                     && (!product.AvailableEndDateTimeUtc.HasValue
                         || product.AvailableEndDateTimeUtc >= nowUtc)
               select product;
    }

    private void SetDiscountAmountPercentageForCategory(int categoryId, PriceRangeFilterDto priceRangeModel)
    {
        var maxDiscountAmount = 0m;
        var maxDiscountPercentage = 0m;

        foreach (var discount in GetAllowedDiscountsForCategory(categoryId))
        {
            maxDiscountAmount = Math.Max(maxDiscountAmount, discount.DiscountAmount);
            maxDiscountPercentage = Math.Max(maxDiscountPercentage, discount.DiscountPercentage);
        }

        priceRangeModel.MaxDiscountAmount = maxDiscountAmount;
        priceRangeModel.MaxDiscountPercentage = maxDiscountPercentage;
    }

    private IEnumerable<Discount> GetAllowedDiscountsForCategory(int categoryId)
    {
        return new List<Discount>();
    }
}
