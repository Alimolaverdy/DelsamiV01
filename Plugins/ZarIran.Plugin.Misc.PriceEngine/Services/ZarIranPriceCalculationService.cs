using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.ProductManagements.Services;

namespace ZarIran.Plugin.Misc.PriceEngine.Services
{
    public class ZarIranPriceCalculationService : IZarIranPriceCalculationService
    {
        private readonly IProductService _productService;
        private readonly CoreSettings _coreSettings;
        private readonly IProductPricingService _productPricingService;



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

            return (ProductPriceRounding.RoundUp(oldPrice), ProductPriceRounding.RoundUp(price));
        }
    }
}
