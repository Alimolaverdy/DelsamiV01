using Delsami.Plugin.Payment.Core.Models;

namespace NopFarsi.Payments.Pasargad.Models.PasargadPayments
{
    public class PasargadPayResultModel : BasePaymentResponseModel
    {
        public string Status { get; set; }

        public int InvoiceId { get; set; }

        public int TrackId { get; set; }

        public string ReferenceNumber { get; set; }


        public override string ToString()
        {
            return $"Status:{Status} - invoiceId:{InvoiceId} - trackId:{TrackId} - referenceNumber:{ReferenceNumber}";
        }
    }
}
