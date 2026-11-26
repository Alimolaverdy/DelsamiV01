using Delsami.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core.Events;
using Nop.Services.ScheduleTasks;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Events;
using System;

namespace Delsami.Plugin.Misc.ExtendedOrder.ScheduleTasks
{
    public class SendOrderTrackingSMSScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly IExtendedOrderService _extendedOrderService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public SendOrderTrackingSMSScheduleTask(IExtendedOrderService extendedOrderService,
                                                IEventPublisher eventPublisher)
        {
            _extendedOrderService = extendedOrderService;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            try
            {
                var orders = await _extendedOrderService.GetExceptDelsamiLogisticProcessingOrdersAfterThreeDaysAsync();

                foreach (var order in orders)
                {
                    var orderProccessingAfterThreeDaysEvent = new OrderProccessingAfterThreeDaysEvent(order);

                    await _eventPublisher.PublishAsync(orderProccessingAfterThreeDaysEvent);
                }
            }
            catch
            {

            }
        }

        #endregion
    }
}
