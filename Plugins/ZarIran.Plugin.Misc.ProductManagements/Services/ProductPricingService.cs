using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    internal class ProductPricingService : IProductPricingService
    {

        private readonly IRepository<ProductPricing> _productPricingRepo;
        private readonly IProductService _productService;
        private readonly CoreSettings _coreSettings;

        public ProductPricingService(IRepository<ProductPricing> productPricingRepo, IProductService productService, CoreSettings coreSettings)
        {
            _productPricingRepo = productPricingRepo;
            _productService = productService;
            _coreSettings = coreSettings;
        }

        public async Task<ProductPricing> GetByProductIdAsync(int productId)
        {
            return await _productPricingRepo.Table.FirstOrDefaultAsync(w => w.ProductId == productId);
        }

        public async Task InsertAsync(ProductPricing zarIranProductPriceDetailsModel)
        {
            if (zarIranProductPriceDetailsModel is null)
                throw new ArgumentNullException(nameof(zarIranProductPriceDetailsModel));

            await _productPricingRepo.InsertAsync(zarIranProductPriceDetailsModel);
        }

        public async Task UpdateAsync(ProductPricing zarIranProductPriceDetailsModel)
        {
            await _productPricingRepo.UpdateAsync(zarIranProductPriceDetailsModel);
        }


    }
}
