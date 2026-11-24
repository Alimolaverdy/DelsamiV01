using System;
using Nop.Core;

namespace ZarIran.Plugin.Misc.GoldPurchase.Domain;

public class GoldWithdrawal : BaseEntity
{
    public Guid RequestKey { get; set; }
    public int CustomerId { get; set; }
    public int StoreId { get; set; }
    public string CustomerName { get; set; }
    public string Phone { get; set; }
    public string Sheba { get; set; }
    public long Amount { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? CompletedOnUtc { get; set; }
    public int? CompletedByCustomerId { get; set; }
}
