using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.TorobApi.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Misc.TorobApi.Configuration.LogEnabled")]
        public bool LogEnabled { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.TorobApi.Configuration.LogEnabledOrderRequest")]
        public bool LogEnabledOrderRequest { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.TorobApi.Configuration.TorobPublicKey")]
        public string TorobPublicKey { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Misc.TorobApi.Configuration.IsActive")]
        public bool IsActive { get; set; }

    }
}
