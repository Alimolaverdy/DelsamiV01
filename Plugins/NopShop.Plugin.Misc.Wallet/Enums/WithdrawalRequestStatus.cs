using System.ComponentModel;

namespace NopShop.Plugin.Misc.Wallet.Enums
{
    public enum WithdrawalRequestStatus
    {
        [Description("NopShop.Plugin.Misc.Wallet.Enums.WithdrawalRequestStatus.Approved")]
        Approved = 1,

        [Description("NopShop.Plugin.Misc.Wallet.Enums.WithdrawalRequestStatus.Pending")]
        Pending = 1,

        [Description("NopShop.Plugin.Misc.Wallet.Enums.WithdrawalRequestStatus.InProgress")]
        InProgress,

        [Description("NopShop.Plugin.Misc.Wallet.Enums.WithdrawalRequestStatus.Rejected")]
        Rejected
    }
}
