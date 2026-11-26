using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Newtonsoft.Json;

namespace Delsami.Plugin.Payments.Barook.Models.Barook
{
	public class StartPaymentResponse
	{
		[JsonProperty("token")]
		public string Token { get; set; }

		[JsonProperty("timestamp")]
		public DateTime Timestamp { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("externalCode")]
		public string ExternalCode { get; set; }

		[JsonProperty("terminalNumber")]
		public string terminalNumber { get; set; }

		[JsonProperty("terminalCode")]
		public string TerminalCode { get; set; }

		[JsonProperty("details")]
		public Dictionary<string, object> details { get; set; }

	}
}
