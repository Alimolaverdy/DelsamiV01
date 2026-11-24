namespace ZarIran.Plugin.Payments.Digipay.Models.DigiPays
{
    public class AdditionalInfoModel
    {
        public long prepaymentAmount { get; set; }
        public long cashAmount { get; set; }
        public long creditAmount { get; set; }
        public bool instantFinalization { get; set; }
        public bool generateInvoice { get; set; }
    }
}
