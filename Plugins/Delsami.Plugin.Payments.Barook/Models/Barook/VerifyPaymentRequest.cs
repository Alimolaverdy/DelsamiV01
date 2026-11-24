using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ZarIran.Plugin.Payments.Barook.Models.Barook
{
	public class VerifyPaymentRequest
	{
		[JsonProperty("terminalCode")]
		public string TerminalCode { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }

		[JsonProperty("externalCode")]
		public string ExternalCode { get; set; }

		[JsonProperty("token")]
		public string Token { get; set; }
	}



}
