using ZarIran.Plugin.Payment.Core.Models;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ZarIran.Plugin.Payments.Digipay.Models.DigiPays;

public class VerifyResponseModel : BasePaymentResponseModel
{
    public string trackingCode { get; set; }
    public string providerId { get; set; }
    public string fpCode { get; set; }
    public string fpName { get; set; }
    public string amount { get; set; }
    public PaymentGateway paymentGateway { get; set; }
    public ResultModel result { get; set; }
    public AdditionalInfoModel additionalInfo { get; set; }

    public override string ToString()
    {
        var option = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        return $"trackingCode:{trackingCode} - providerId:{providerId} - fpCode:{fpCode} - fpName:{fpName} - amount:{amount} - paymentGateway:{paymentGateway.ToString()} -" +
               $" result: {JsonSerializer.Serialize(result, option)} - additionalInfo: {JsonSerializer.Serialize(additionalInfo, option)}";
    }
}
