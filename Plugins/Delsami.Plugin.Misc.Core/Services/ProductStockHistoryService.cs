using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Services
{
    public class ProductStockHistoryService : IProductStockHistoryService
    {
        private readonly IProductService _productService;

        public ProductStockHistoryService(
            IProductService productService)
        {
            _productService = productService;
        }

        public async Task<int> GetPreviousStockQuantityAsync(Product product)
        {
            var history = await _productService.GetStockQuantityHistoryAsync(product);

            var previousRecord = history
                .OrderByDescending(h => h.CreatedOnUtc)
                .Skip(1)
                .FirstOrDefault();

            return previousRecord?.StockQuantity ?? 0;
        }
    }
}
