using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.GoldPurchase.Models;

/// <summary>Model of the "Gold wallet" card on the admin customer page.</summary>
public record CustomerGoldWalletModel : BaseNopModel
{
    public string CustomerName { get; set; }
    public int HistoryPage { get; set; } = 1;
    public int HistoryPages { get; set; }
    public int CustomerId { get; set; }
    public decimal BalanceGrams { get; set; }
    public IList<Row> Rows { get; set; } = new List<Row>();

    public record Row
    {
        public DateTime CreatedOn { get; set; }
        public string Type { get; set; }
        public decimal Grams { get; set; }
        public decimal BalanceAfterGrams { get; set; }
        public decimal PricePerGram { get; set; }
        public decimal Amount { get; set; }
        public int? AdminCustomerId { get; set; }
        public int? OrderId { get; set; }
        public string Note { get; set; }
    }
}
