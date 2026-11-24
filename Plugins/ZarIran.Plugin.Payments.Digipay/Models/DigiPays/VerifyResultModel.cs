using ZarIran.Plugin.Payment.Core.Models;

namespace ZarIran.Plugin.Payments.Digipay.Models.DigiPays
{
    public class VerifyResultModel : BasePaymentResponseModel
    {
        public long amount { get; set; }
        public string providerId { get; set; }
        public string trackingCode { get; set; }
        public string rrn { get; set; }
        public int? psp { get; set; }
        public bool isCredit { get; set; }
        public PaymentResult result { get; set; }
        public int type { get; set; }

        public override string ToString()
        {
            return $"amount: {amount} - providerId: {providerId} - trackingCode: {trackingCode} - rrn: {rrn} - psp: {psp} - isCredit: {isCredit} - result: {result} - type: {type}";
        }
    }
}
