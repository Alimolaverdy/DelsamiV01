using System.ComponentModel;

namespace ZarIran.Plugin.Shipping.Tipax.Enums
{
    public enum PaymentType
    {
        [Description("سمت فرستنده-اعتباری")]
        SenderCredit = 10,

        [Description("سمت گیرنده-پسکرایه")]
        ReceiverCashOnDelivery = 20,

        [Description("سمت گیرنده-پرداخت در محل")]
        ReceiverPaymentOnDelivery = 30,

        [Description("سمت فرستنده-پرداخت در محل")]
        SenderPaymentOnDelivery = 40,

        [Description("سمت فرستنده-پرداخت از کیف پول")]
        SenderWalletPayment = 50,

        [Description("سمت فرستنده-نقدی")]
        SenderCash = 80
    }
}
