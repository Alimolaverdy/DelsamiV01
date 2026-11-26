using Delsami.Plugin.Payment.Core.Models;
using System;

namespace NopFarsiIR.Payments.Mellat.Models
{
    public class MellatSettleResponseModel : BasePaymentResponseModel
    {
        public string ResultCode { get; set; }

        public override string ToString()
        {
            return $"resultCode:{ResultCode}";
        }
    }
}
