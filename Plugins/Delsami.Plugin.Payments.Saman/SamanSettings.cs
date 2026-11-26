using Nop.Core.Configuration;

namespace Delsami.Plugin.Payments.Saman
{
    public class SamanSettings : ISettings
    {
        public string TerminalNumber { get; set; }

        public string TerminalPassword { get; set; }

        public string BaseUrl { get; set; }

        public int TimeOutSeconds { get; set; } = 30;

        public int TokenCacheMinutes { get; set; } = 20;

        public string DescriptionText { get; set; }

    }
}
