namespace Delsami.Plugin.Payments.TorobPay
{
    public static class TorobPayPaymentDefaults
    {
        public const string OAuthTokenEndpoint = "/api/online/v1/oauth/token";
        public const string EligibleEndpoint = "/api/online/offer/v1/eligible";
        public const string PaymentTokenEndpoint = "/api/online/payment/v1/token";
        public const string VerifyEndpoint = "/api/online/payment/v1/verify";
        public const string SettleEndpoint = "/api/online/payment/v1/settle";
        public const string RevertEndpoint = "/api/online/payment/v1/revert";
        public const string StatusEndpoint = "/api/online/payment/v1/status";
        public const string CancelEndpoint = "/api/online/payment/v1/cancel";
        public const string UpdateEndpoint = "/api/online/payment/v1/update";
    }
}