using DocumentFormat.OpenXml.Spreadsheet;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public class GroupedProductStockService : IGroupedProductStockService
    {
        private readonly IProductService _productService;
        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly ICustomDelsamiProductService _customDelsamiProductService;

        public GroupedProductStockService(IProductService productService, IVirtualProductMappingService virtualProductMappingService, ICustomDelsamiProductService customDelsamiProductService)
        {
            _productService = productService;
            _virtualProductMappingService = virtualProductMappingService;
            _customDelsamiProductService = customDelsamiProductService;
        }

        public async Task RecalculateParentStockAsync(int parentGroupedProductId)
        {
            if (parentGroupedProductId == 0)
                return;

            var parentProduct = await _productService.GetProductByIdAsync(parentGroupedProductId);
            if (parentProduct == null)
                return;

            var children = await _productService.GetAssociatedProductsAsync(parentGroupedProductId);

            int totalStock = 0;
            var countedMainIds = new HashSet<int>();

            if (children != null)
            {
                foreach (var child in children)
                {
                    int mainId;
                    int stockToAdd;

                    //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
                    //DeliveryDateId is 'MainProductId'

                    if (child.AllowAddingOnlyExistingAttributeCombinations && child.DeliveryDateId > 0)
                    {
                        var mainProduct = await _productService.GetProductByIdAsync(child.DeliveryDateId);

                        if (mainProduct is null)
                            throw new NullReferenceException(nameof(mainProduct));

                        if (!mainProduct.Published || mainProduct.Deleted)
                            continue;

                        mainId = mainProduct.Id;
                        stockToAdd = mainProduct.StockQuantity;
                    }
                    else
                    {
                        if (!child.Published || child.Deleted)
                            continue;

                        mainId = child.Id;
                        stockToAdd = child.StockQuantity;
                    }

                    if (!countedMainIds.Contains(mainId))
                    {
                        totalStock += stockToAdd;
                        countedMainIds.Add(mainId);
                    }
                }
            }

            parentProduct.StockQuantity = totalStock;
            await _productService.UpdateProductAsync(parentProduct);
        }


        public async Task UpdateParentStockIncrementalAsync(int childProductId, int quantityDelta)
        {
            if (quantityDelta == 0)
                return;

            var updatedParentIds = new HashSet<int>();

            var mainProduct = await _productService.GetProductByIdAsync(childProductId);

            //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
            //DeliveryDateId is 'MainProductId'
            if (mainProduct.AllowAddingOnlyExistingAttributeCombinations && mainProduct.DeliveryDateId > 0)
                mainProduct = await _productService.GetProductByIdAsync(mainProduct.DeliveryDateId);

            if (mainProduct == null)
                return;

            var virtualProducts = await _customDelsamiProductService.GetVirtualProductsByMainProductIdAsync(mainProduct.Id);

            if (virtualProducts != null && virtualProducts.Any())
            {
                foreach (var vp in virtualProducts)
                {
                    var parentId = vp.ParentGroupedProductId;

                    if (parentId == 0 || updatedParentIds.Contains(parentId))
                        continue;

                    var parentProduct =
                        await _productService.GetProductByIdAsync(parentId);

                    if (parentProduct == null)
                        continue;

                    parentProduct.StockQuantity += quantityDelta;

                    await _productService.UpdateProductAsync(parentProduct);

                    updatedParentIds.Add(parentId);
                }
            }

            var mainParent =
                await _productService.GetProductByIdAsync(mainProduct.ParentGroupedProductId);

            if (mainParent != null && !updatedParentIds.Contains(mainParent.Id))
            {
                mainParent.StockQuantity += quantityDelta;

                await _productService.UpdateProductAsync(mainParent);
            }
        }
    }
}
