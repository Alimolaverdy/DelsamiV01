using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Payments.Digipay.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.Username")]
        public string Username { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.GrantType")]
        public string GrantType { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.ClientId")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.ClientSecret")]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.BaseUrl")]
        public string BaseUrl { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.DigiPayVersion")]
        public string DigiPayVersion { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.DigiPay.Configuration.DescriptionText")]
        public string DescriptionText { get; set; }
    }
}
