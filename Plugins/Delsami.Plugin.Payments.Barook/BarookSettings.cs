using Nop.Core.Configuration;

namespace ZarIran.Plugin.Payments.Barook
{
	public class BarookSettings : ISettings
    {
		public string TerminalCode { get; set; }

		public string TerminalPassword { get; set; }

		public string BaseUrl { get; set; }

		public string businessServiceSlug { get; set; }

		public int paymentDailyCount { get; set; }



	}
}
