using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.Digipay.Models.DigiPays
{
    public class DeliverRequestModel
    {
        public long deliveryDate { get; set; }

        public string invoiceNumber { get; set; }

        public string trackingCode { get; set; }

        public List<string> products { get; set; }
    }
}
