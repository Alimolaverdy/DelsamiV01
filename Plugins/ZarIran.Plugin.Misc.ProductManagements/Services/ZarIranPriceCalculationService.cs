using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class ZarIranPriceCalculationService : IZarIranPriceCalculationService
    {

        private readonly IProductService _productService;
        private readonly CoreSettings _coreSettings;
        private readonly IProductPricingService _productPricingService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;

        public ZarIranPriceCalculationService(IProductService productService,
                                              CoreSettings coreSettings,
                                              IProductPricingService productPricingService,
                                              IWorkContext workContext,
                                              ICustomerService customerService)
        {
            _productService = productService;
            _coreSettings = coreSettings;
            _productPricingService = productPricingService;
            _workContext = workContext;
            _customerService = customerService;
        }

        public async Task<(decimal Oldprice, decimal Price)> GetCalculatedProductPriceAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
                throw new ArgumentException($"Product with ID {productId} was not found.", nameof(productId));

            if (product.ProductType != Nop.Core.Domain.Catalog.ProductType.SimpleProduct)
                return (0m, 0m);

            var productInfo = await _productPricingService.GetByProductIdAsync(productId);

            if (productInfo == null)
                throw new InvalidOperationException($"Pricing information for product {productId} was not found.");

            // بررسی رول مشتری جاری
            var customer = await _workContext.GetCurrentCustomerAsync();

            var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);

            bool isCoWorker = _coreSettings.CoWorkerRoleId > 0 &&
                              customerRoleIds.Contains(_coreSettings.CoWorkerRoleId);

            // انتخاب درصدهای مناسب
            decimal makingFeePercent = isCoWorker
                ? productInfo.CoWorkerMakingFee
                : productInfo.MakingFee;

            decimal profitPercent = isCoWorker
                ? productInfo.CoWorkerProfitPercent
                : productInfo.ProfitPercent;

            decimal oldMakingFeePercent = isCoWorker
                ? productInfo.OldCoWorkerMakingFee
                : productInfo.OldMakingFee;

            decimal oldProfitPercent = isCoWorker
                ? productInfo.OldCoWorkerProfitPercent
                : productInfo.OldProfitPercent;

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

            decimal makingFee = currentGoldPrice * makingFeePercent / 100m;

            decimal profit = (currentGoldPrice + makingFee) * profitPercent / 100m;

            decimal tax = (profit + makingFee + productInfo.AdditionalAmount) * taxPercent / 100m;

            decimal price = currentGoldPrice + makingFee + profit + tax + productInfo.AdditionalAmount;

            // ==========================================
            // 2. قیمت قبلی
            // ==========================================

            decimal oldGoldPrice = weight * goldRate;

            decimal oldMakingFee = oldGoldPrice * oldMakingFeePercent / 100m;

            decimal oldProfit = (oldGoldPrice + oldMakingFee) * oldProfitPercent / 100m;

            decimal oldTax = (oldProfit + oldMakingFee + productInfo.AdditionalAmount) * taxPercent / 100m;

            decimal oldPrice = oldGoldPrice + oldMakingFee + oldProfit + oldTax + productInfo.AdditionalAmount;

            return (ProductPriceRounding.RoundUp(oldPrice), ProductPriceRounding.RoundUp(price));
        }
    }
}