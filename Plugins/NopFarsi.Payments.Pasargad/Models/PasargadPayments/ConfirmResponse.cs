using ZarIran.Plugin.Payment.Core.Models;
using System.Text.Json;

namespace NopFarsi.Payments.Pasargad.Models.PasargadPayments;

public class ConfirmResponse : BasePaymentResponseModel
{
    public string resultMsg { get; set; }

    public int resultCode { get; set; }

    public ConfirmResponseData data { get; set; }

    public override string ToString()
    {
        return $"resultMsg:{resultMsg}, resultCode:{resultCode}, data:{JsonSerializer.Serialize(data)}";
    }
}
