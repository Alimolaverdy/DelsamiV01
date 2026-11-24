using ZarIran.Plugin.Payment.Core.Enums;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Models.Apis
{
    public class InsertPaymentRequestModel
    {
        public string ShipmentCode { get; set; }

        public string ReferenceNumber { get; set; }

        public string TrackId { get; set; }

        public decimal Amount { get; set; }

        public PaymentGateway Gateway { get; set; }

        public string PoseCode { get; set; }

        public int PoseId { get; set; }
    }
}
