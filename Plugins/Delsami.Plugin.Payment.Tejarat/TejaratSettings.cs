using Nop.Core.Configuration;

namespace ZarIran.Plugin.Payment.Tejarat
{
    public class TejaratSettings : ISettings
    {
        public string TerminalId { get; set; }

        public string AcceptorId { get; set; }

        public string PassPhrase { get; set; }

        public string RsaPublicKey { get; set; }

    }
}
