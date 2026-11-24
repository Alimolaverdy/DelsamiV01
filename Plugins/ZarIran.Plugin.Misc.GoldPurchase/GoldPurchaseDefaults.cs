namespace ZarIran.Plugin.Misc.GoldPurchase;

public static class GoldPurchaseDefaults
{
    public const string WithdrawalRoute = "ZarIran.Plugin.Misc.GoldPurchase.Withdrawal";
    public const string ChargeRoute = "ZarIran.Plugin.Misc.GoldPurchase.Buy";
    public const string SellRoute = "ZarIran.Plugin.Misc.GoldPurchase.Sell";
    public const string OrderAmountAttribute = "ZarIran.GoldPurchase.Amount";
    public const string OrderGoldPriceAttribute = "ZarIran.GoldPurchase.PricePerGram";
    public const string OrderGoldWeightAttribute = "ZarIran.GoldPurchase.GoldWeightGrams";

    // This is intentionally a string so the plugin does not need a hard project
    // reference to the Core plugin just to read its setting.
    public const string CoreGoldPriceSettingKey =
        "ZarIran.Plugin.Misc.Core.Configuration.ManualGoldPrice";
}
