using DocumentFormat.OpenXml.Spreadsheet;
using ZarIran.Plugin.Misc.Core.Services;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class GroupedProductStockService : IGroupedProductStockService
    {
        private readonly IProductService _productService;
        private readonly IVirtualProductMappingService _virtualProductMappingService;

        public GroupedProductStockService(IProductService productService, IVirtualProductMappingService virtualProductMappingService)
        {
            _productService = productService;
            _virtualProductMappingService = virtualProductMappingService;
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

                    var mainResult = await _virtualProductMappingService
                        .GetMainProductByVirtualProductIdAsync(child.Id);

                    if (mainResult.IsVirtualPrdocut && mainResult.MainProduct != null)
                    {
                        var mainProduct = mainResult.MainProduct;

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

            var mapping = await _virtualProductMappingService.GetMainProductByVirtualProductIdAsync(childProductId);

            Product mainProduct;

            if (mapping.IsVirtualPrdocut && mapping.MainProduct is not null)
                mainProduct = mapping.MainProduct;
            else
                mainProduct = await _productService.GetProductByIdAsync(childProductId);

            if (mainProduct == null)
                return;

            var virtualMappings =
                await _virtualProductMappingService.GetVirtualProductMappingsByMainProductIdAsync(mainProduct.Id);

            if (virtualMappings != null)
            {
                foreach (var vm in virtualMappings)
                {
                    var virtualProduct =
                        await _productService.GetProductByIdAsync(vm.VirtualProductId);

                    if (virtualProduct == null)
                        continue;

                    var parentId = virtualProduct.ParentGroupedProductId;

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
