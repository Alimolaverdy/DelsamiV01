using Nop.Core;
using System;

namespace ZarIran.Plugin.Misc.GoldPurchase.Domain;

public class GoldWallet : BaseEntity
{
    public int CustomerId { get; set; }
    public decimal GoldBalanceGrams { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}
