namespace ZarIran.Plugin.Payment.Tejarat.Models.Payments
{
    public class TokenResult
    {
        public TokenResult()
        {
            result = new Result();
        }

        public string responseCode { get; set; }

        public object description { get; set; }

        public bool status { get; set; }

        public Result result { get; set; }
    }
}
