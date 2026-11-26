using Delsami.Plugin.Payment.Core.Enums;
using System;

namespace Delsami.Plugin.Misc.ShipmentManagement.Models.Apis
{
    public class InsertCODPaymentRequestModel
    {
        public bool HasDeliveryCode { get; set; }
        public int? DevliveryCode { get; set; }
        public string ShipmentCode { get; set; }
        public string ReferenceNumber { get; set; }
        public string TrackingId { get; set; }
        public string TerminalNumber { get; set; }
        public decimal Amount { get; set; }
        public PaymentGateway Gateway { get; set; }
        public DateTime PayDate{ get; set; }
    }
}
