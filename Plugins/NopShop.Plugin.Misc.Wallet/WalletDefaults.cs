using Nop.Core;
using Nop.Core.Caching;

namespace NopShop.Plugin.Misc.Wallet
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class WalletDefaults
    {
        /// <summary>
        /// Gets a name of the route to the import contacts callback
        /// </summary>
        public static string ChargeWalletRoute => "NopShop.Plugin.Misc.Wallet.ChargeWallet";

        /// <summary>
        /// Generic attribute name to hide general settings block on the plugin configuration page
        /// </summary>
        public static string HideGeneralBlock = "WalletPage.HideGeneralBlock";
    }
}