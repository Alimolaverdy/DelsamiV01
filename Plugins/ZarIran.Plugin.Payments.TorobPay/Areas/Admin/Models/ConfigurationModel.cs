using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Payments.TorobPay.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.ClientId")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.ClientSecret")]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.Username")]
        public string Username { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.BaseUrl")]
        public string BaseUrl { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.TokenCacheMinutes")]
        public int TokenCacheMinutes { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.MinimumAllowedAmount")]
        public decimal MinimumAllowedAmount { get; set; }

        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.MaximumAllowedAmount")]
        public decimal MaximumAllowedAmount { get; set; }


        [NopResourceDisplayName("ZarIran.Plugin.Payments.TorobPay.Configuration.TimeoutSeconds")]
        public int TimeoutSeconds { get; set; }
    }
}
