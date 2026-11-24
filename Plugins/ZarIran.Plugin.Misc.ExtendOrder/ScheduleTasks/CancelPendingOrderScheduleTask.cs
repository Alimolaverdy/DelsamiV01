using ZarIran.Plugin.Misc.Core.Events;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.ScheduleTasks
{
    public class CancelPendingOrderScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ExtendedOrderSettings _extendedOrderSettings;
        private readonly ISubStatusService _subStatusService;
        private readonly ISubStatusMappingService _subStatusMappingService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public CancelPendingOrderScheduleTask(IOrderService orderService,
                                       IOrderProcessingService orderProcessingService,
                                       ExtendedOrderSettings extendedOrderSettings,
                                       ISubStatusService subStatusService,
                                       ISubStatusMappingService subStatusMappingService,
                                       IEventPublisher eventPublisher)
        {
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _extendedOrderSettings = extendedOrderSettings;
            _subStatusService = subStatusService;
            _subStatusMappingService = subStatusMappingService;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var orderStatus = new List<int>() { (int)OrderStatus.Pending };

            var toDateTimeUtc = DateTime.UtcNow.AddMinutes(-(_extendedOrderSettings.OrderExpirationAsMinutes));

            var pendingOrders = await _orderService.SearchOrdersAsync(osIds: orderStatus, createdToUtc: toDateTimeUtc);

            foreach (var pendingOrder in pendingOrders)
            {
                await _orderProcessingService.CancelOrderAsync(pendingOrder, false);

                //Adding subStatus
                var subStatus = await _subStatusService.GetOrderSubStatusAsync(_extendedOrderSettings.systemSubStatusId);
                if (subStatus != null)
                {
                    var subStatusMappingService = new OrderSubStatusMapping
                    {
                        OrderId = pendingOrder.Id,
                        SubStatusId = subStatus.Id,
                        CreatedDateTime = DateTime.UtcNow,
                    };

                    await _subStatusMappingService.InsertAsync(subStatusMappingService);
                }

                var pendingOrderCanceled = new PendingOrderCanceledEvent(pendingOrder.Id);

                await _eventPublisher.PublishAsync(pendingOrderCanceled);
            }
        }

        #endregion
    }
}
