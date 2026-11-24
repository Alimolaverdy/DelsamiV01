using Nop.Core.Configuration;

namespace ZarIran.Plugin.Misc.TorobApi
{
    public class TorobApiSettings : ISettings
    {
        public bool LogEnabled { get; set; }

        public bool LogEnabledOrderRequest { get; set; }

        public string TorobPublicKey { get; set; }

        public bool IsActive { get; set; }
    }
}
