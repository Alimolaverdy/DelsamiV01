using ZarIran.Plugin.Payment.Core.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ZarIran.Plugin.Payment.Tejarat.Models.Payments;

public class VerifyResult : BasePaymentResponseModel
{
    public string responseCode { get; set; }

    public string description { get; set; }

    public bool status { get; set; }

    public SubResult result { get; set; }

    public override string ToString()
    {
        var option = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        return $"responseCode: {responseCode} - description: {description} - status: {status} - result: {JsonSerializer.Serialize(result, option)}";
    }
}
