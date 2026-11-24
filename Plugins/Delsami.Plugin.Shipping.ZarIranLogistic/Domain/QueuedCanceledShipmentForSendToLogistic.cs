using Nop.Core;
using System;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Domain
{
    public class QueuedCanceledShipmentForSendToLogistic : BaseEntity
    {
        public int ShipmentId { get; set; }

        public bool Sent { get; set; }

        public int SentTries { get; set; }

        public string Result { get; set; }

        public int? ResultStatusCode { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public DateTime? lastTryDateTime { get; set; }

        public DateTime? SentDateTime { get; set; }

    }
}
