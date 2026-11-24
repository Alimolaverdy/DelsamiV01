using System.Text.Json.Serialization;

namespace ZarIran.Plugin.Payments.Barook.Models.Barook
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum TransactionStatusEnum
	{
		CREATED,
		HOLDED,
		PENDING,
		COMPLETED,
		CANCELED,
		CLOSED,
		FRAUD,
		TEST
	}



}
