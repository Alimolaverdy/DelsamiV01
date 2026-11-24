using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ZarIran.Plugin.Payments.Barook.Models.Barook
{
	public class SaleItemDto
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("saleCount")]
		public int SaleCount { get; set; }

		[JsonProperty("unitType")]
		public string UnitType { get; set; }

		[JsonProperty("amount")]
		public decimal Amount { get; set; }
		
		[JsonProperty("totalAmount")]
		public decimal TotalAmount { get; set; }

		[JsonProperty("sku")]
		public string Sku { get; set; }
	}



}
