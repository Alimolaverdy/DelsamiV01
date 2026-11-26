namespace Delsami.Plugin.Payments.Saman
{
    public static class SamanPaymentEndpoints
    {
        public const string GetToken = "/onlinepg/onlinepg";
        public const string Purchase = "/onlinepg/sendtoken?token=";
        public const string Verify = "/verifyTxnRandomSessionkey/ipg/VerifyTransaction";
    }
}
