using Nop.Core.Configuration;

namespace ZarIran.Plugin.Payments.Digipay
{
    public class DigiPaySettings : ISettings
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string GrantType { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string BaseUrl { get; set; }

        public string DigiPayVersion { get; set; }

        public string DescriptionText { get; set; }
    }
}
