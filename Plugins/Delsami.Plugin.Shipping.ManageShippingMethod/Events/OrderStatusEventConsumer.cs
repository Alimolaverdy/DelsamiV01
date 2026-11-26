using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Events
{
    public class OrderStatusEventConsumer : IConsumer<OrderStatusChangedEvent>
    {
        public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
        {
            var status = eventMessage.Order.OrderStatus;

            if (status == OrderStatus.Complete)
            {
            }

            if (status == OrderStatus.Cancelled)
            {
            }

            await Task.CompletedTask;
        }
    }
}
