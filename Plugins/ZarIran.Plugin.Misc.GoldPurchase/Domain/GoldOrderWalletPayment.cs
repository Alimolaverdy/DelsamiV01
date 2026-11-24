using Nop.Core;

namespace ZarIran.Plugin.Misc.GoldPurchase.Domain;

public class GoldOrderWalletPayment : BaseEntity
{
    public int OrderId { get; set; }
    public int Points { get; set; }
    public int SalePoints { get; set; }
    public bool Refunded { get; set; }
}
