namespace ZarIran.Plugin.Payments.Digipay.Models.DigiPays
{
    public class RefundRequestModel
    {
        public string amount { get; set; }

        public string providerId { get; set; }

        public string saleTrackingCode { get; set; }
    }
}
