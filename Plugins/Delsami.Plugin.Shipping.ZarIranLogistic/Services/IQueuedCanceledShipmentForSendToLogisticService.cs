using ZarIran.Plugin.Shipping.ZarIranLogistic.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Services
{
    public interface IQueuedCanceledShipmentForSendToLogisticService
    {
        Task InsertAsync(QueuedCanceledShipmentForSendToLogistic queuedCanceledShipmentForSendToLogistic);

        Task<IList<QueuedCanceledShipmentForSendToLogistic>> GetNotSentQueuedCanceledShipmentForSendToLogisticsAsync();

        Task UpdateAsync(QueuedCanceledShipmentForSendToLogistic queuedCanceledShipmentForSendToLogistic);

    }
}
