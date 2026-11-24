using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Payment.Core.Models
{
    public record CheckoutFailedModel : BaseNopModel
    {
        public int? OrderId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
