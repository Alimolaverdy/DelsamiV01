using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Delsami.Plugin.Payment.Tejarat.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Delsami.Plugin.Payment.Tejarat.Configuration.TerminalId")]
        public string TerminalId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Tejarat.Configuration.AcceptorId")]
        public string AcceptorId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Tejarat.Configuration.PassPhrase")]
        public string PassPhrase { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Payment.Tejarat.Configuration.RsaPublicKey")]
        public string RsaPublicKey { get; set; }
    }
}
