using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Payments.Digipay.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
    }
}