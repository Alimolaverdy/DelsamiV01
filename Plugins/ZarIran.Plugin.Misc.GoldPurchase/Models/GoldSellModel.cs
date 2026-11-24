using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Misc.GoldPurchase.Models;

public record GoldSellModel : BaseNopModel
{
    public decimal WalletBalanceGrams { get; set; }

    /// <summary>Current market price per gram (Core plugin setting).</summary>
    public decimal MarketPricePerGram { get; set; }

    /// <summary>What the customer actually gets per gram (market price minus commission).</summary>
    public decimal SellPricePerGram { get; set; }

    public decimal CommissionPercent { get; set; }

    public decimal GoldWeightGrams { get; set; }

    public string QuoteBy { get; set; } = "weight";

    public decimal? TargetAmount { get; set; }

    public string SuccessMessage { get; set; }
}
