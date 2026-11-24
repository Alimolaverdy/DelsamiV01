using Nop.Core;
using System;
using ZarIran.Plugin.Misc.GoldPurchase.Enums;

namespace ZarIran.Plugin.Misc.GoldPurchase.Domain;

/// <summary>
/// One row of the gold ledger (same idea as nopCommerce's RewardPointsHistory).
/// GoldWeightGrams is SIGNED: positive = credit, negative = debit.
/// </summary>
public class GoldTransaction : BaseEntity
{
    public int CustomerId { get; set; }
    public int? OrderId { get; set; }
    public decimal GoldWeightGrams { get; set; }
    public decimal PricePerGram { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal GoldBalanceAfterGrams { get; set; }
    public string Note { get; set; }
    public int? AdminCustomerId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
