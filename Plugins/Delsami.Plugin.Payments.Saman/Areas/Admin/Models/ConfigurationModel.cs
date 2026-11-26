using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Payments.Saman.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Delsami.Plugin.Payments.Saman.Configuration.TerminalNumber")]
        public string TerminalNumber { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Saman.Configuration.TerminalPassword")]
        public string TerminalPassword { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Saman.Configuration.BaseUrl")]
        public string BaseUrl { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Saman.Configuration.DescriptionText")]
        public string DescriptionText { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Saman.Configuration.TimeOutSeconds")]
        public int TimeOutSeconds { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payments.Saman.Configuration.TokenCacheMinutes")]
        public int TokenCacheMinutes { get; set; }

    }
}
