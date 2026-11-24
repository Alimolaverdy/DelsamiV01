using Nop.Core.Configuration;

namespace NopShop.Plugin.Misc.Wallet.Domain
{
    public class WalletSettings : ISettings
    {
        public int WalletProductId { get; set; }

        public int Amount { get; set; }

        public string PaymentSystemName { get; set; }
    }
}
