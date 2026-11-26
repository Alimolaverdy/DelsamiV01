using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Events
{
    public class OrderStatusCompletedHandler : IConsumer<OrderStatusChangedEvent>
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IShipmentService _shipmentService;

        #endregion

        #region Ctor

        public OrderStatusCompletedHandler(IOrderService orderService,
                                           IEventPublisher eventPublisher,
                                           IShipmentService shipmentService)
        {
            _orderService = orderService;
            _eventPublisher = eventPublisher;
            _shipmentService = shipmentService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
        {
            var order = eventMessage.Order;

            if (eventMessage.Order != null && eventMessage.Order.OrderStatus == OrderStatus.Complete)
            {
                order.ShippingStatusId = (int)ShippingStatus.Delivered;

                if (order.PaymentStatus != PaymentStatus.Paid)
                {
                    order.PaymentStatus = PaymentStatus.Paid;

                    if (!order.PaidDateUtc.HasValue)
                        order.PaidDateUtc = DateTime.UtcNow;
                }
                await _orderService.UpdateOrderAsync(order);

                var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(order.Id);

                foreach (var shipment in shipments)
                {
                    shipment.DeliveryDateUtc = DateTime.UtcNow;

                    await _shipmentService.UpdateShipmentAsync(shipment);

                    await _eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
                }
            }
        }

        #endregion
    }
}
