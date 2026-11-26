namespace Delsami.Plugin.Payments.Barook
{
    public static class BarookPaymentEndpoints
    {
        public const string GetToken = "/barook/cpg/v2/start-payment";
        public const string Purchase = "/barook/cpg/redirect/";
        public const string Verify = "/barook/cpg/v2/verify-payment";
    }
}
