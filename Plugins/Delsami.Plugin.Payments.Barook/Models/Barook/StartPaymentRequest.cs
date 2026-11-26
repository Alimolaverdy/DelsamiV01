using System.Collections.Generic;
using Newtonsoft.Json;

namespace Delsami.Plugin.Payments.Barook.Models.Barook
{

	public class StartPaymentRequest
	{
		[JsonProperty("terminalCode")]
		public string TerminalCode { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }

		[JsonProperty("totalAmount")]
		public long TotalAmount { get; set; }

		[JsonProperty("externalCode")]
		public string ExternalCode { get; set; }

		[JsonProperty("paymentDailyCount")]
		public int paymentDailyCount { get; set; }

		[JsonProperty("ownerName")]
		public string OwnerName { get; set; }

		[JsonProperty("ownerMobile")]
		public string OwnerMobile { get; set; }

		[JsonProperty("ownerNationalCode")]
		public string OwnerNationalCode { get; set; }

		[JsonProperty("redirectUrl")]
		public string RedirectUrl { get; set; }

		[JsonProperty("saleItemDto")]
		public List<SaleItemDto> SaleItemDto { get; set; }

		[JsonProperty("branchCode")]
		public string BranchCode { get; set; }

		[JsonProperty("businessServiceSlug")]
		public string BusinessServiceSlug { get; set; }
		
		[JsonProperty("attributes")]
		public Dictionary<string, object> Attributes { get; set; }
	}


}
