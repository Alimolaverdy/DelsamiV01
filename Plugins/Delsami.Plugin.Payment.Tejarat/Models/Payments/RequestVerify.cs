using System.Runtime.Serialization;

namespace Delsami.Plugin.Payment.Tejarat.Models.Payments
{
    public class RequestVerify
    {
        //شماره ترمینال
        public string terminalId { get; set; }

        [DataMember]
        //شماره ارجاع
        public string retrievalReferenceNumber { get; set; }

        [DataMember]
        //شماره سند/ پیگیری
        public string systemTraceAuditNumber { get; set; }

        [DataMember]
        //توکن
        public string tokenIdentity { get; set; }

        [DataMember]
        //مقدار
        public long Amount { get; set; }

        [DataMember]
        ////ریسپانس کد
        public string responseCode { get; set; }
    }
}
