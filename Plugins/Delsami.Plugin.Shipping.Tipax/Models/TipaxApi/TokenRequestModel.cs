namespace ZarIran.Plugin.Shipping.Tipax.Models.TipaxApi
{
    public record TokenRequestModel
    {
        public string username { get; set; }

        public string password { get; set; }

        public string apiKey { get; set; }
    }
}
