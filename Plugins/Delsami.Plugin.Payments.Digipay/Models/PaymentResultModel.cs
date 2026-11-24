using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Payments.Digipay.Models
{
    public record PaymentResultModel : BaseNopModel
    {
        public bool IsSucessful { get; set; }

        public string TrackingCode { get; set; }

        public string Message { get; set; }
    }
}
