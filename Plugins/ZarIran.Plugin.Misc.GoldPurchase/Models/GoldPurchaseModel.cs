using Nop.Web.Framework.Models;

namespace ZarIran.Plugin.Misc.GoldPurchase.Models;

public record GoldPurchaseModel : BaseNopModel
{
    public string QuoteBy { get; set; } = "amount";
    public decimal CashWalletBalance { get; set; }
    public bool UseCashWallet { get; set; }
    public decimal? CashWalletAmount { get; set; }
    public string[] Shebas { get; set; } = System.Array.Empty<string>();
    public GoldSellModel Sell { get; set; } = new();
    public Services.GoldPortfolio Portfolio { get; set; } = new();
    public string ActiveTab { get; set; } = "buy";
    public string SuccessMessage { get; set; }
    public decimal Amount { get; set; }
    public decimal BuyPricePerGram { get; set; }
    public decimal GoldPricePerGram { get; set; }
    public decimal GoldWeightGrams { get; set; }
    public decimal MinimumAmount { get; set; }
    public decimal MaximumAmount { get; set; }
    public string PaymentSystemName { get; set; }
    public decimal BuyCommission { get; set; }
    public decimal SellCommission { get; set; }
    public bool IsActive { get; set; }
    public decimal WalletBalanceGrams { get; set; }
}
