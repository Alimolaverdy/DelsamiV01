namespace NopFarsiIR.Plugin.Payments.BankPasargad.Models.PasargadPayments
{
    public class purchaseRequest
    {
        public purchaseRequest()
        {
            serviceType = "PURCHASE";
        }

        public long amount { get; set; }

        public string callbackApi { get; set; }

        public string invoice { get; set; }

        public string invoiceDate { get; set; }

        public int serviceCode { get; set; }

        public int terminalNumber { get; set; }

        public string serviceType { get; set; }

        public string mobileNumber { get; set; }

    }
}