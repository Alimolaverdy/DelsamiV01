using System.Collections.Generic;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services
{
    public interface IGoldWalletService
    {
        Task<GoldWallet> GetOrCreateWalletAsync(int customerId);

        /// <summary>Current balance in grams (0 when the customer has no wallet yet).</summary>
        Task<decimal> GetBalanceAsync(int customerId);

        /// <summary>Newest first.</summary>
        Task<IList<GoldTransaction>> GetHistoryAsync(int customerId, int count = 100, int pageIndex = 0);
        Task<int> GetHistoryCountAsync(int customerId);
        Task<GoldPortfolio> GetPortfolioAsync(int customerId, decimal marketPrice);

        /// <summary>Credits a paid purchase order. Idempotent per order.</summary>
        Task<GoldTransaction> CreditPurchaseAsync(int customerId, int orderId, decimal amount, decimal pricePerGram, decimal goldWeightGrams);

        /// <summary>
        /// Customer sells grams back. Debits the wallet atomically (never below zero) and writes a Sell ledger row
        /// and credits the proceeds to the monetary wallet. Publishes <see cref="Events.GoldSoldEvent"/>.
        /// </summary>
        Task<GoldOperationResult> SellAsync(int customerId, decimal grams, decimal marketPricePerGram, decimal sellCommissionPercent, int storeId = 0);

        /// <summary>
        /// Admin correction: positive grams add, negative grams subtract (never below zero).
        /// </summary>
        Task<GoldOperationResult> AdjustAsync(int customerId, decimal gramsDelta, string note, int adminCustomerId);
    }
}
