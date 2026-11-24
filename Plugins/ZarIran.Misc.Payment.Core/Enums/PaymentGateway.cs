using System.ComponentModel;

namespace ZarIran.Plugin.Payment.Core.Enums
{
    public enum PaymentGateway
    {
        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Pasargad")]
        Pasargad = 1,

        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Tejarat")]
        Tejarat,

        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Mellat")]
        Mellat,

        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Torob")]
        Torob,

        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Saman")]
        Saman,

        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Resalat")]
        Resalat,

        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Digipay")]
        Digipay,

        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Tara")]
        Tara,
        
        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.DigiKala")]
        DigiKala,
        
        [Description("ZarIran.Plugin.Payment.Core.PaymentGateway.Barook")]
        Barook,
    }
}
