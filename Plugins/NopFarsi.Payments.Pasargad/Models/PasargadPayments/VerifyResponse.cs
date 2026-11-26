using Delsami.Plugin.Payment.Core.Models;

namespace NopFarsiIR.Plugin.Payments.BankPasargad.Controller;

public class VerifyResponse : BasePaymentResponseModel
{
    public string resultMsg { get; set; }

    public int resultCode { get; set; }

    public DataModel data { get; set; }

    public override string ToString()
    {
        return $"resultMsg:{resultMsg}, resultCode:{resultCode},data:{data}";
    }

    public class DataModel
    {
        public string invoice { get; set; }

        public string referenceNumber { get; set; }

        public string trackId { get; set; }

        public string maskedCardNumber { get; set; }

        public string hashedCardNumber { get; set; }

        public string requestDate { get; set; }

        public double amount { get; set; }
    }
}
