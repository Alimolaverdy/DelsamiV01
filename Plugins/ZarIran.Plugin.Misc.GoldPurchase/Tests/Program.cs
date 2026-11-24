using ZarIran.Plugin.Misc.GoldPurchase.Services;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;
using ZarIran.Plugin.Misc.GoldPurchase.Enums;

static GoldTransaction Row(int id, decimal grams, decimal amount, TransactionType type = TransactionType.Purchase)
    => new() { Id = id, CreatedOnUtc = new DateTime(2026, 1, 1).AddDays(id), GoldWeightGrams = grams, Amount = amount, TransactionType = type };
static void Equal(decimal expected, decimal actual, string name) { if (Math.Abs(expected - actual) > 0.000001m) throw new Exception($"{name}: expected {expected}, got {actual}"); }
static void True(bool value, string name) { if (!value) throw new Exception(name); }
var purchase = new[] { Row(1, 2, 2000) };
var portfolio = GoldPortfolio.Calculate(purchase, 2, 1200);
Equal(2000, portfolio.TotalPaid, "Paid amount"); Equal(400, portfolio.ProfitLoss, "Rising market"); Equal(20, portfolio.ProfitPercent.Value, "Gain percent");
Equal(-400, GoldPortfolio.Calculate(purchase, 2, 800).ProfitLoss, "Falling market");
Equal(0, GoldPortfolio.Calculate(purchase, 2, 1000).ProfitLoss, "Unchanged market");
var partialSale = new[] { Row(2, -1, 1100, TransactionType.Sell), Row(1, 2, 2000) }; // Deliberately unsorted
portfolio = GoldPortfolio.Calculate(partialSale, 1, 1200);
Equal(1000, portfolio.RemainingCost, "Partial sale removes proportional cost"); Equal(200, portfolio.ProfitLoss, "Sale is not a loss"); Equal(1100, portfolio.SaleProceeds, "Sale proceeds");
portfolio = GoldPortfolio.Calculate(partialSale.Append(Row(3, 1, 1400)), 2, 1300);
Equal(2400, portfolio.RemainingCost, "Rebuy at different price"); Equal(200, portfolio.ProfitLoss, "Moving average");
portfolio = GoldPortfolio.Calculate(partialSale.Append(Row(3, -1, 1200, TransactionType.Sell)), 0, 1300);
Equal(0, portfolio.RemainingCost, "Complete sale clears cost"); Equal(0, portfolio.ProfitLoss, "Empty wallet no unrealized loss");
portfolio = GoldPortfolio.Calculate(purchase.Append(Row(2, 1, 0, TransactionType.AdminAdjustment)), 3, 1000);
Equal(2000, portfolio.RemainingCost, "Manual credit no customer payment"); True(portfolio.HasManualCredits, "Manual credit disclosure");
portfolio = GoldPortfolio.Calculate(purchase.Append(Row(2, -1, 0, TransactionType.AdminAdjustment)), 1, 1000);
Equal(1000, portfolio.RemainingCost, "Manual debit removes proportional cost");
True(!GoldPortfolio.Calculate(purchase, 5, 1000).IsComplete, "Missing ledger detected");
True(!GoldPortfolio.Calculate(new[] { Row(1, -1, 1000, TransactionType.Sell) }, 0, 1000).IsComplete, "Invalid historical debit detected");
portfolio = GoldPortfolio.Calculate(Enumerable.Range(1, 150).Select(i => Row(i, 1, 1000)), 150, 1200);
Equal(150000, portfolio.TotalPaid, "History beyond 100 rows"); Equal(30000, portfolio.ProfitLoss, "Complete history valuation");
portfolio = GoldPortfolio.Calculate(Array.Empty<GoldTransaction>(), 0, 1000);
Equal(0, portfolio.ProfitLoss, "New wallet"); True(portfolio.ProfitPercent == null && portfolio.IsComplete, "Zero cost no division by zero");
Equal(950, GoldPricing.GetSellPricePerGram(1000, 5), "Commission"); Equal(285, GoldPricing.GetSellAmount(.3m, 950), "Sale net amount");
Equal(.12345678m, GoldPricing.RoundGrams(.123456789m), "Eight decimal precision");
Console.WriteLine("PASS: portfolio gains/losses, partial/full sales, rebuy, admin credit/debit, missing ledger, >100 transactions, commission and rounding.");

Equal(1050, GoldPricing.GetBuyPricePerGram(1000, 5), "Buy commission included in effective quote");
Equal(1000, GoldPricing.GetBuyPricePerGram(1000, 0), "Zero buy fee");
Equal(1050.105m, GoldPricing.GetBuyPricePerGram(1000.1m, 5), "Per-gram quote stays precise");
Equal(2, GoldPricing.RoundGrams(2100 / GoldPricing.GetBuyPricePerGram(1000, 5)), "Paid amount includes fee, not additional charge");
Equal(1000, GoldPricing.GetSellPricePerGram(1000, 0), "Zero sell fee");
Equal(0, GoldPricing.GetSellPricePerGram(1000, 100), "Full sell fee has no payout");
Console.WriteLine("PASS: configured buy/sell commissions, inclusive purchase amount, zero fees and quote rounding.");

Equal(26250000, GoldPricing.GetBuyAmount(1, GoldPricing.GetBuyPricePerGram(25000000, 5)), "One gram uses spot plus fee");
Equal(65625000, GoldPricing.GetBuyAmount(2.5m, GoldPricing.GetBuyPricePerGram(25000000, 5)), "Exact entered grams determine purchase amount");
Equal(24500000, GoldPricing.GetSellAmount(1, GoldPricing.GetSellPricePerGram(25000000, 2)), "One gram sale uses spot minus fee");
Equal(10502, GoldPricing.GetBuyAmount(10, GoldPricing.GetBuyPricePerGram(1000.1m, 5)), "Round final buy total, not each gram");
Console.WriteLine("PASS: spot gold by exact entered grams, buy/sell fees and final-total rounding.");
