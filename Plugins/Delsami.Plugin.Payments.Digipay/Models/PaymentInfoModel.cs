using Nop.Web.Framework.Models;

namespace Delsami.Plugin.Payments.Digipay.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
    }
}