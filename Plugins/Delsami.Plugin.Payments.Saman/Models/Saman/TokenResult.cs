using Newtonsoft.Json;

namespace ZarIran.Plugin.Payments.Saman.Models.Saman;

public class TokenResult
{
    [JsonProperty("status")]
    public int StatusCode { get; set; }

    [JsonProperty("errorCode")]
    public int ErrorCode { get; set; }

    [JsonProperty("errorDesc")]
    public string ErrorDescription { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }
}
