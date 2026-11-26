using Delsami.Plugin.Payment.Core.Models;

namespace NopFarsiIR.Payments.Mellat.Models
{
    public class MellatVerifyResponseModel : BasePaymentResponseModel
    {
        public string ResultCode { get; set; }

        public override string ToString()
        {
            return $"resultCode:{ResultCode}";
        }
    }
}
