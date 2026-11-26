using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public class ReservedStockQuantityService : IReservedStockQuantityService
    {
        #region Fields

        private readonly IRepository<ReservedStockQuantity> _reservedStockQuantityRepo;
        private readonly IProductService _productService;
        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly IOrderService _orderService;
        private readonly IReservedStockQuantityHistoryService _reservedStockQuantityHistory;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ReservedStockQuantityService(IRepository<ReservedStockQuantity> reservedStockQuantityRepo,
                                            IProductService productService,
                                            IVirtualProductMappingService virtualProductMappingService,
                                            IOrderService orderService,
                                            IReservedStockQuantityHistoryService reservedStockQuantityHistory,
                                            ILocalizationService localizationService)
        {
            _reservedStockQuantityRepo = reservedStockQuantityRepo;
            _productService = productService;
            _virtualProductMappingService = virtualProductMappingService;
            _orderService = orderService;
            _reservedStockQuantityHistory = reservedStockQuantityHistory;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task<int> GetReservedStockQuantityCountAsync(int productId)
        {
            //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
            //DeliveryDateId is 'MainProductId'
            var mainProductId = (await _productService.GetProductByIdAsync(productId))?.DeliveryDateId;

            if (mainProductId == 0)
                mainProductId = productId;

            var reservedStockQuantity = await GetReservedStockQuantityAsync(mainProductId.Value);

            if (reservedStockQuantity is null)
                return 0;

            return reservedStockQuantity.Count;
        }

        public async Task<ReservedStockQuantity> GetReservedStockQuantityAsync(int productId)
        {
            return await _reservedStockQuantityRepo.Table.FirstOrDefaultAsync(f => f.ProductId == productId);
        }

        public async Task InsertAsync(ReservedStockQuantity reservedStockQuantity)
        {
            if (reservedStockQuantity == null)
                throw new ArgumentNullException(nameof(reservedStockQuantity));

            await _reservedStockQuantityRepo.InsertAsync(reservedStockQuantity);
        }

        public async Task UpdateAsync(ReservedStockQuantity reservedStockQuantity)
        {
            if (reservedStockQuantity == null)
                throw new ArgumentNullException(nameof(reservedStockQuantity));

            reservedStockQuantity.LastModifiedDateTime = DateTime.UtcNow;

            await _reservedStockQuantityRepo.UpdateAsync(reservedStockQuantity);
        }

        public async Task AdjustReservedStockQuantityAsync(Product product, string orderId, int quantity, string message)
        {
            Product mainProduct = null;
            //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
            //DeliveryDateId is 'MainProductId'
            if (product.AllowAddingOnlyExistingAttributeCombinations)
                mainProduct = await _productService.GetProductByIdAsync(product.DeliveryDateId);
            else
                mainProduct = product;

            if (mainProduct is null)
                throw new NullReferenceException(nameof(mainProduct));

            var reservedStockQuantity = await GetReservedStockQuantityAsync(mainProduct.Id);

            if (reservedStockQuantity is null)
            {
                reservedStockQuantity = new ReservedStockQuantity
                {
                    ProductId = mainProduct.Id,
                    LastModifiedDateTime = DateTime.UtcNow
                };

                await InsertAsync(reservedStockQuantity);
            }

            reservedStockQuantity.Count += quantity;

            await UpdateAsync(reservedStockQuantity);
            await _reservedStockQuantityHistory.InsertAsync(mainProduct, reservedStockQuantity, orderId, quantity, message);
        }

        public async Task ReverseReservedStockQuantityAsync(Order order, string message)
        {
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            foreach (var orderItem in orderItems)
            {
                await AdjustReservedStockQuantityAsync(orderItem.ProductId, order.Id, -orderItem.Quantity, message);
            }
        }

        #endregion

        #region Utilities

        private async Task AdjustReservedStockQuantityAsync(int productId, int orderId, int quantity, string message)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product is null)
                throw new ArgumentNullException(nameof(product));

            await AdjustReservedStockQuantityAsync(product, orderId.ToString(), quantity, message);
        }

        #endregion
    }
}
