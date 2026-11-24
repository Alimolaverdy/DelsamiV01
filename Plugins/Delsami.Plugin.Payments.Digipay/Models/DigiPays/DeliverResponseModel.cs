using ZarIran.Plugin.Payment.Core.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ZarIran.Plugin.Payments.Digipay.Models.DigiPays;

public class DeliverResponseModel : BasePaymentResponseModel
{
    public ResultModel result { get; set; }

    public override string ToString()
    {
        var option = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        return $"result: {JsonSerializer.Serialize(result, option)}";
    }
}
