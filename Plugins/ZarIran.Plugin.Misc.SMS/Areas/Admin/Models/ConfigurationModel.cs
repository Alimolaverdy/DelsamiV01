using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ZarIran.Plugin.Misc.SMS.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {

        [NopResourceDisplayName("ZarIran.Plugin.Misc.SMS.Configuration.ApiKey")]
        public string Apikey { get; set; }


        [NopResourceDisplayName("ZarIran.Plugin.Misc.SMS.Configuration.SenderNumber")]
        public string SenderNumber { get; set; }


        [NopResourceDisplayName("ZarIran.Plugin.Misc.SMS.Configuration.MaximumLengthOfSMS")]
        public int MaximumLengthOfSMS { get; set; }


        [NopResourceDisplayName("ZarIran.Plugin.Misc.SMS.Configuration.BaseUrl")]
        public string BaseUrl { get; set; }
    }
}
