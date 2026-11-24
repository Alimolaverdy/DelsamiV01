using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.ProductManagements.Services;

namespace ZarIran.Plugin.Misc.PriceEngine.Services
{
    public class e : PriceCalculationService, IPriceCalculationService
    {
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICustomerService _customerService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductService _productService;
        private readonly CoreSettings _coreSettings;
        private readonly IProductPricingService _productPricingService;

        public e(CatalogSettings catalogSettings,
                                              CurrencySettings currencySettings,
                                              CoreSettings coreSettings,
                                              ICategoryService categoryService,
                                              IProductPricingService productPricingService,
                                              ICurrencyService currencyService,
                                              ICustomerService customerService,
                                              IDiscountService discountService,
                                              IManufacturerService manufacturerService,
                                              IProductAttributeParser productAttributeParser,
                                              IProductService productService,
                                              IStaticCacheManager staticCacheManager) : base(catalogSettings, currencySettings, categoryService, currencyService, customerService, discountService, manufacturerService, productAttributeParser, productService, staticCacheManager)
        {
            _productPricingService = productPricingService;
            _coreSettings = coreSettings;
            _productService = productService;
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _staticCacheManager = staticCacheManager;
        }

        public virtual async Task<(decimal priceWithoutDiscounts, decimal finalPrice, decimal appliedDiscountAmount, List<Discount> appliedDiscounts)> GetFinalPriceAsync(Product product,
                   Customer customer,
                   Store store,
                   decimal? overriddenProductPrice,
                   decimal additionalCharge,
                   bool includeDiscounts,
                   int quantity,
                   DateTime? rentalStartDate,
                   DateTime? rentalEndDate)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductPriceCacheKey,
                product,
                overriddenProductPrice,
                additionalCharge,
                includeDiscounts,
                quantity,
                await _customerService.GetCustomerRoleIdsAsync(customer),
                store);

            //we do not cache price if this not allowed by settings or if the product is rental product
            //otherwise, it can cause memory leaks (to store all possible date period combinations)
            if (!_catalogSettings.CacheProductPrices || product.IsRental)
                cacheKey.CacheTime = 0;

            decimal rezPrice;
            decimal rezPriceWithoutDiscount;
            decimal discountAmount;
            List<Discount> appliedDiscounts;

            (rezPriceWithoutDiscount, rezPrice, discountAmount, appliedDiscounts) = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var discounts = new List<Discount>();

                //initial price
                var price = overriddenProductPrice ?? product.Price;

                //tier prices
                var tierPrice = await _productService.GetPreferredTierPriceAsync(product, customer, store, quantity);

                if (tierPrice != null)
                    price = tierPrice.Price;

                var (_, calculatedPrice) = await GetCalculatedProductPriceAsync(product.Id);

                price = calculatedPrice;

                //additional charge
                price += additionalCharge;

                //rental products
                if (product.IsRental)
                    if (rentalStartDate.HasValue && rentalEndDate.HasValue)
                        price *= _productService.GetRentalPeriods(product, rentalStartDate.Value, rentalEndDate.Value);

                var priceWithoutDiscount = price;

                if (includeDiscounts)
                {
                    //discount
                    var (tmpDiscountAmount, tmpAppliedDiscounts) = await GetDiscountAmountAsync(product, customer, price);
                    price -= tmpDiscountAmount;

                    if (tmpAppliedDiscounts?.Any() ?? false)
                    {
                        discounts.AddRange(tmpAppliedDiscounts);
                    }
                }

                if (price < decimal.Zero)
                    price = decimal.Zero;

                if (priceWithoutDiscount < decimal.Zero)
                    priceWithoutDiscount = decimal.Zero;

                var roundedPriceWithoutDiscount = ProductPriceRounding.RoundUp(priceWithoutDiscount);
                var roundedPrice = ProductPriceRounding.RoundUp(price);
                return (roundedPriceWithoutDiscount, roundedPrice,
                    roundedPriceWithoutDiscount - roundedPrice, discounts);
            });

            return (rezPriceWithoutDiscount, rezPrice, discountAmount, appliedDiscounts);
        }

        public async Task<(decimal Oldprice, decimal Price)> GetCalculatedProductPriceAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
                throw new ArgumentException($"Product with ID {productId} was not found.", nameof(productId));

            if (product.ProductType != Nop.Core.Domain.Catalog.ProductType.SimpleProduct)
            {
                return (0m, 0m);
            }

            var productInfo = await _productPricingService.GetByProductIdAsync(productId);

            if (productInfo == null)
                throw new InvalidOperationException($"Pricing information for product {productId} was not found.");

            decimal weight = product.Weight;
            decimal goldRate = _coreSettings.ManualGoldPrice;
            decimal taxPercent = _coreSettings.Tax;

            // ==========================================
            // 1. قیمت فعلی
            // ==========================================

            decimal currentGoldRate = goldRate;

            if (_coreSettings.AllowPriceAdjustment)
                currentGoldRate -= _coreSettings.GoldPriceAdjustment;

            decimal currentGoldPrice = weight * currentGoldRate;

            decimal makingFee = currentGoldPrice * productInfo.MakingFee / 100m;

            decimal profit = (currentGoldPrice + makingFee) * productInfo.ProfitPercent / 100m;

            decimal tax = (profit + makingFee) * taxPercent / 100m;

            decimal price = currentGoldPrice + makingFee + profit + tax;

            // ==========================================
            // 2. قیمت قبلی
            // بدون کسر GoldPriceAdjustment
            // ==========================================

            decimal oldGoldPrice = weight * goldRate;

            decimal oldMakingFee = oldGoldPrice * productInfo.OldMakingFee / 100m;

            decimal oldProfit = (oldGoldPrice + oldMakingFee) * productInfo.OldProfitPercent / 100m;

            decimal oldTax = (oldProfit + oldMakingFee) * taxPercent / 100m;

            decimal oldPrice = oldGoldPrice + oldMakingFee + oldProfit + oldTax;

            return (oldPrice, price);
        }
    }
}
