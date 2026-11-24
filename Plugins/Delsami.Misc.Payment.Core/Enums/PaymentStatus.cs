using System.ComponentModel;

namespace ZarIran.Plugin.Payment.Core.Enums
{
    public enum PaymentStatus
    {
        [Description("ZarIran.Plugin.Payment.Core.PaymentStatus.Confirmed")]
        Confirmed = 1,

        [Description("ZarIran.Plugin.Payment.Core.PaymentStatus.Verified")]
        Verified,

        [Description("ZarIran.Plugin.Payment.Core.PaymentStatus.Initial")]
        Initial,

        [Description("ZarIran.Plugin.Payment.Core.PaymentStatus.InProgress")]
        InProgress,

        [Description("ZarIran.Plugin.Payment.Core.PaymentStatus.Failed")]
        Failed,

        [Description("ZarIran.Plugin.Payment.Core.PaymentStatus.Refunded")]
        Refunded
    }
}
