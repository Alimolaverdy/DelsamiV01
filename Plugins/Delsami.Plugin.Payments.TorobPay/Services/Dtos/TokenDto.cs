namespace ZarIran.Plugin.Payments.TorobPay.Services.Dtos
{
    public record TokenDto(string username, string password);

    public class OAuthTokenResponse
    {
        public string access_token { get; set; }
    }
}
