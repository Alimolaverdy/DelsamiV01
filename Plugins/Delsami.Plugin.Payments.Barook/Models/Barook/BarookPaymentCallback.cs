using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Delsami.Plugin.Payment.Core.Models;

namespace Delsami.Plugin.Payments.Barook.Models.Barook
{
    public class BarookPaymentCallback : BasePaymentResponseModel
	{
		[JsonProperty("Token")]
		public string Token { get; set; }

		[JsonProperty("externalCode")]
		public string ExternalCode { get; set; }

		[JsonProperty("cardNumber")]
		public string CardNumber { get; set; }

		[JsonProperty("status")]
		public TransactionStatusEnum Status { get; set; }

        public override string ToString()
        {
			return $"Token: {Token} - ExternalCode: {ExternalCode} - CardNumber: {CardNumber} - Status: {Status}";
        }
    }



}
