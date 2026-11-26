using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.ShipmentManagement.Domain
{
    public class ShipmentDelivery : BaseEntity
    {
        public int ShipmentId { get; set; }

        public int DeliveryCode { get; set; }

        public bool Delivered { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public bool Shipped { get; set; }

        public DateTime? ShippedOnUtc { get; set; }

        public DateTime? DeliveredOnUtc { get; set; }
    }
}
