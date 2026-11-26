namespace Delsami.Plugin.Payment.Tejarat.Infrastructure
{
    public class Constant
    {
        public const string GetTokenUrl = "https://ikc.shaparak.ir/api/v3/tokenization/make";
        public const string PaymentUrl = "https://ikc.shaparak.ir/iuiv3/IPG/Index";
        public const string VerifyUrl = "https://ikc.shaparak.ir/api/v3/confirmation/purchase";
        public const string InquiryUrl = "https://ikc.shaparak.ir/api/v3/inquiry/single";
    }
}
