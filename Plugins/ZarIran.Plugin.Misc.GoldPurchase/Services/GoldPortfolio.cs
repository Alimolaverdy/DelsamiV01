using System;
using System.Collections.Generic;
using System.Linq;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;
using ZarIran.Plugin.Misc.GoldPurchase.Enums;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

/// <summary>Moving-average cost of remaining holdings; sales do not count as investment losses.</summary>
public class GoldPortfolio
{
    public decimal TotalPaid { get; private set; }
    public decimal SaleProceeds { get; private set; }
    public decimal RemainingCost { get; private set; }
    public decimal CurrentValue { get; private set; }
    public decimal ProfitLoss => CurrentValue - RemainingCost;
    public decimal? ProfitPercent => RemainingCost > 0 ? ProfitLoss / RemainingCost * 100 : null;
    public bool HasManualCredits { get; private set; }
    public bool IsComplete { get; private set; } = true;

    public static GoldPortfolio Calculate(IEnumerable<GoldTransaction> history, decimal balance, decimal marketPrice)
    {
        var result = new GoldPortfolio();
        decimal grams = 0, cost = 0;
        foreach (var row in history.OrderBy(x => x.CreatedOnUtc).ThenBy(x => x.Id))
        {
            if (row.GoldWeightGrams > 0)
            {
                grams += row.GoldWeightGrams;
                if (row.TransactionType == TransactionType.Purchase)
                {
                    result.TotalPaid += row.Amount;
                    cost += row.Amount;
                }
                else result.HasManualCredits = true; // No customer payment for a manual credit.
            }
            else if (row.GoldWeightGrams < 0)
            {
                var removed = -row.GoldWeightGrams;
                if (removed > grams) result.IsComplete = false;
                cost = grams > 0 ? cost * Math.Max(0, grams - removed) / grams : 0;
                grams = Math.Max(0, grams - removed);
                if (row.TransactionType == TransactionType.Sell) result.SaleProceeds += row.Amount;
            }
        }
        if (Math.Abs(grams - balance) > 0.00000001m) result.IsComplete = false;
        result.RemainingCost = cost;
        result.CurrentValue = balance * marketPrice;
        return result;
    }
}
