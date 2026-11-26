namespace Delsami.Plugin.Payments.Digipay.Models.DigiPays
{
    public class GetTokenResponseModel
    {
        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public string token_type { get; set; }

        public string expires_in { get; set; }
    }
}
