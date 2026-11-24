using Nop.Core.Configuration;

namespace ZarIran.Plugin.Misc.GoldPurchase;

public class GoldPurchaseSettings : ISettings
{
    public int GoldProductId { get; set; }
    public decimal DefaultAmount { get; set; }
    public decimal MinimumAmount { get; set; }
    public decimal MaximumAmount { get; set; }
    public decimal BuyCommission { get; set; }
    public decimal SellCommission { get; set; }
    public bool IsActive { get; set; }
    public string PaymentSystemName { get; set; }
}
