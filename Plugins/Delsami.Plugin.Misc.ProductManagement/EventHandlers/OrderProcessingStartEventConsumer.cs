using Delsami.Plugin.Misc.Core.Events;
using Delsami.Plugin.Misc.ProductManagement.Services;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.EventHandlers
{
    public class OrderProcessingStartEventConsumer : IConsumer<OrderProcessingStartEvent>
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly IReservedStockQuantityService _reservedStockQuantityService;

        #endregion

        #region Ctor

        public OrderProcessingStartEventConsumer(IProductService productService,
                                                 ILocalizationService localizationService,
                                                 IOrderService orderService,
                                                 IVirtualProductMappingService virtualProductMappingService,
                                                 IReservedStockQuantityService reservedStockQuantityService)
        {
            _productService = productService;
            _localizationService = localizationService;
            _orderService = orderService;
            _virtualProductMappingService = virtualProductMappingService;
            _reservedStockQuantityService = reservedStockQuantityService;
        }

        #endregion

        public async Task HandleEventAsync(OrderProcessingStartEvent eventMessage)
        {
            var orderItems = await _orderService.GetOrderItemsAsync(eventMessage.Order.Id);

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                if (product is null)
                    throw new ArgumentNullException(nameof(product));

                Product mainProduct = null;

                //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
                //DeliveryDateId is 'MainProductId'
                if (product.AllowAddingOnlyExistingAttributeCombinations)
                    mainProduct = await _productService.GetProductByIdAsync(product.DeliveryDateId);
                else
                    mainProduct = product;

                await _productService.AdjustInventoryAsync(mainProduct, -orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.PlaceOrder"), eventMessage.Order.Id));

                await _reservedStockQuantityService.AdjustReservedStockQuantityAsync(product,
                                                                                     eventMessage.Order.Id.ToString(),
                                                                                     -orderItem.Quantity,
                                                                                    string.Format(await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ProductManagement.Admin.ReservedStockQuantityHistory.Messages.OrderProcessingStart"), eventMessage.Order.Id));

            }
        }
    }
}
