using System.Collections.Generic;

namespace ZarIran.Plugin.Payment.Tejarat.Models.Payments
{
    public class IPGDataModel
    {


        // کد پایانه 
        //[XmlElement("TerminalId")]
        public string TerminalId { get; set; }

        // کد پذیرنده 
        public string AcceptorId { get; set; }
        // عبارت عبور
        public string PassPhrase { get; set; }
        // مقدار-ریال
        public long Amount { get; set; }
        //{

        //    get { return amount; }

        //    set { amount = long.Parse(amount) }

        //}
        //Hard Code ip in xml file
        public string RevertURL { get; set; }
        public string PaymentId { get; set; }
        public string RequestId { get; set; }
        public string CmsPreservationId { get; set; }
        public string TransactionType { get; set; }

        public BillInfo BillInfo { get; set; }

        public string RsaPublicKey { get; set; }
        public List<MultiplexParameter> MultiplexParameters { get; set; }



    }
}
