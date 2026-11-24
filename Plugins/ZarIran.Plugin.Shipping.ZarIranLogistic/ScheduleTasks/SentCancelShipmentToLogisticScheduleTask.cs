using ZarIran.Plugin.Shipping.ZarIranLogistic.Services;
using Nop.Core.Domain.Orders;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.ScheduleTasks
{
    public class SentCancelShipmentToLogisticScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly IQueuedCanceledShipmentForSendToLogisticService _queuedCanceledShipmentForSendToLogisticService;
        private readonly IZarIranLogisticApiService _ZarIranLogisticApiService;

        #endregion

        #region Ctor

        public SentCancelShipmentToLogisticScheduleTask(IQueuedCanceledShipmentForSendToLogisticService queuedCanceledShipmentForSendToLogisticService,
                                                        IZarIranLogisticApiService ZarIranLogisticApiService)
        {
            _queuedCanceledShipmentForSendToLogisticService = queuedCanceledShipmentForSendToLogisticService;
            _ZarIranLogisticApiService = ZarIranLogisticApiService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var orderStatus = new List<int>() { (int)OrderStatus.Pending };


            var notSentQueuedCanceledShipmentForSendToLogistics = await _queuedCanceledShipmentForSendToLogisticService.GetNotSentQueuedCanceledShipmentForSendToLogisticsAsync();

            foreach (var queuedCanceledShipmentForSendToLogistic in notSentQueuedCanceledShipmentForSendToLogistics)
            {
                var result = await _ZarIranLogisticApiService.CloseCargoAndOrderBySellSiteAsync(queuedCanceledShipmentForSendToLogistic.ShipmentId);

                var now = DateTime.UtcNow;

                queuedCanceledShipmentForSendToLogistic.SentTries += 1;
                queuedCanceledShipmentForSendToLogistic.lastTryDateTime = now;
                queuedCanceledShipmentForSendToLogistic.Result = result.message;
                queuedCanceledShipmentForSendToLogistic.ResultStatusCode = result.statusCode;

                if (result.value || result.statusCode == 1002)
                {
                    queuedCanceledShipmentForSendToLogistic.Sent = true;
                    queuedCanceledShipmentForSendToLogistic.SentDateTime = now;
                }

                await _queuedCanceledShipmentForSendToLogisticService.UpdateAsync(queuedCanceledShipmentForSendToLogistic);
            }
        }

        #endregion
    }
}
