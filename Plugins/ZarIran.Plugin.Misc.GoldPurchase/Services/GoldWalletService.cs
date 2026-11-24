using LinqToDB;
using Nop.Data;
using Nop.Core.Events;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;
using ZarIran.Plugin.Misc.GoldPurchase.Enums;
using ZarIran.Plugin.Misc.GoldPurchase.Events;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

public class GoldWalletService : IGoldWalletService
{
    private readonly IRepository<GoldWallet> _walletRepository;
    private readonly IRepository<GoldTransaction> _transactionRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly GoldCashWallet _cashWallet;
    private readonly ICustomerService _customers;

    public GoldWalletService(
        IRepository<GoldWallet> walletRepository,
        IRepository<GoldTransaction> transactionRepository,
        IEventPublisher eventPublisher, GoldCashWallet cashWallet = null, ICustomerService customers = null)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _eventPublisher = eventPublisher;
        _cashWallet = cashWallet;
        _customers = customers;
    }

    #region Reads

    private async Task<GoldWallet> FindWalletAsync(int customerId)
    {
        // OrderBy Id: if a race ever created two wallets, everybody consistently uses the oldest one.
        // GetAllAsync (not FirstOrDefaultAsync) avoids the System.Linq.Async / LinqToDB overload ambiguity.
        var wallets = await _walletRepository.GetAllAsync(q => q
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.Id)
            .Take(1));

        return wallets.FirstOrDefault();
    }

    public async Task<GoldWallet> GetOrCreateWalletAsync(int customerId)
    {
        var wallet = await FindWalletAsync(customerId);

        if (wallet != null)
            return wallet;

        wallet = new GoldWallet
        {
            CustomerId = customerId,
            GoldBalanceGrams = 0,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        await _walletRepository.InsertAsync(wallet);
        return wallet;
    }

    public async Task<decimal> GetBalanceAsync(int customerId)
    {
        var wallet = await FindWalletAsync(customerId);
        return wallet?.GoldBalanceGrams ?? 0m;
    }

    public async Task<IList<GoldTransaction>> GetHistoryAsync(int customerId, int count = 100, int pageIndex = 0)
    {
        return await _transactionRepository.GetAllAsync(q => q
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ThenByDescending(x => x.Id)
            .Skip(Math.Max(0, pageIndex) * count)
            .Take(count));
    }

    public Task<int> GetHistoryCountAsync(int customerId)
        => _transactionRepository.Table.CountAsync(x => x.CustomerId == customerId);

    public async Task<GoldPortfolio> GetPortfolioAsync(int customerId, decimal marketPrice)
    {
        // Entire ledger, not the last 100 rows displayed in the admin card.
        var history = await _transactionRepository.GetAllAsync(q => q.Where(x => x.CustomerId == customerId));
        return GoldPortfolio.Calculate(history, await GetBalanceAsync(customerId), marketPrice);
    }

    #endregion

    #region Balance changes

    // All balance changes go through these two methods. They use a single UPDATE statement
    // ("balance = balance + x" / "balance = balance - x WHERE balance >= x") so two requests
    // running at the same moment can never overwrite each other or spend the same grams twice.

    private async Task CreditAtomicAsync(int walletId, decimal grams)
    {
        var now = DateTime.UtcNow;

        await _walletRepository.Table
            .Where(w => w.Id == walletId)
            .UpdateAsync(w => new GoldWallet
            {
                GoldBalanceGrams = w.GoldBalanceGrams + grams,
                UpdatedOnUtc = now
            });
    }

    /// <returns>false when the balance was not enough (nothing was changed)</returns>
    private async Task<bool> TryDebitAtomicAsync(int walletId, decimal grams)
    {
        var now = DateTime.UtcNow;

        var affected = await _walletRepository.Table
            .Where(w => w.Id == walletId && w.GoldBalanceGrams >= grams)
            .UpdateAsync(w => new GoldWallet
            {
                GoldBalanceGrams = w.GoldBalanceGrams - grams,
                UpdatedOnUtc = now
            });

        return affected > 0;
    }

    private async Task<GoldTransaction> InsertLedgerRowAsync(
        int walletId,
        int customerId,
        int? orderId,
        decimal signedGrams,
        decimal pricePerGram,
        decimal amount,
        TransactionType type,
        string note,
        int? adminCustomerId)
    {
        // Query the table (not GetByIdAsync) so a cached entity can never give a stale balance.
        var rows = await _walletRepository.GetAllAsync(q => q.Where(w => w.Id == walletId));
        var balanceAfter = rows.FirstOrDefault()?.GoldBalanceGrams ?? 0m;

        var transaction = new GoldTransaction
        {
            CustomerId = customerId,
            OrderId = orderId,
            GoldWeightGrams = signedGrams,
            PricePerGram = pricePerGram,
            Amount = amount,
            TransactionType = type,
            GoldBalanceAfterGrams = balanceAfter,
            Note = note,
            AdminCustomerId = adminCustomerId,
            CreatedOnUtc = DateTime.UtcNow
        };

        await _transactionRepository.InsertAsync(transaction);
        return transaction;
    }

    public async Task<GoldTransaction> CreditPurchaseAsync(
        int customerId,
        int orderId,
        decimal amount,
        decimal pricePerGram,
        decimal goldWeightGrams)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        // Idempotency: one successful gold purchase per order.
        var existing = await _transactionRepository.Table
            .FirstOrDefaultAsync(x =>
                x.OrderId == orderId &&
                x.TransactionType == TransactionType.Purchase);

        if (existing != null)
        {
            scope.Complete();
            return existing;
        }

        var grams = GoldPricing.RoundGrams(goldWeightGrams);
        var wallet = await GetOrCreateWalletAsync(customerId);

        await CreditAtomicAsync(wallet.Id, grams);

        var transaction = await InsertLedgerRowAsync(
            wallet.Id, customerId, orderId, grams, pricePerGram, amount,
            TransactionType.Purchase, note: null, adminCustomerId: null);
        scope.Complete();
        return transaction;
    }

    public async Task<GoldOperationResult> SellAsync(
        int customerId,
        decimal grams,
        decimal marketPricePerGram,
        decimal sellCommissionPercent, int storeId = 0)
    {
        if (grams <= 0 || grams > 9999999999m || grams != GoldPricing.RoundGrams(grams))
            return GoldOperationResult.Fail("وزن فروش باید مثبت و حداکثر با ۸ رقم اعشار باشد.");

        var sellPrice = GoldPricing.GetSellPricePerGram(marketPricePerGram, sellCommissionPercent);
        if (sellPrice <= 0)
            return GoldOperationResult.Fail("قیمت فروش طلا معتبر نیست.");

        decimal amount;
        try { amount = GoldPricing.GetSellAmount(grams, sellPrice); }
        catch (OverflowException) { return GoldOperationResult.Fail("مبلغ فروش خارج از محدوده مجاز است."); }

        if (amount <= 0)
            return GoldOperationResult.Fail("مقدار طلا برای فروش بسیار کم است.");

        var points = _cashWallet?.ToPoints(amount) ?? 0;
        if (storeId <= 0 || points <= 0)
            return GoldOperationResult.Fail("این مبلغ با تنظیمات فعلی کیف پول قابل ثبت نیست.");

        var customer = await _customers.GetCustomerByIdAsync(customerId);
        if (customer == null || customer.Deleted)
            return GoldOperationResult.Fail("حساب مشتری پیدا نشد.");

        GoldTransaction transaction;
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var balance = await _cashWallet.GetRewardPointsBalanceAsync(customerId, storeId);
            if (balance > int.MaxValue - points)
                return GoldOperationResult.Fail("موجودی کیف پول پس از فروش از سقف مجاز بیشتر می‌شود.");

            var wallet = await FindWalletAsync(customerId);
            if (wallet == null || !await TryDebitAtomicAsync(wallet.Id, grams))
                return GoldOperationResult.Fail("موجودی طلای شما برای این فروش کافی نیست.");

            transaction = await InsertLedgerRowAsync(
                wallet.Id, customerId, orderId: null, signedGrams: -grams, sellPrice, amount,
                TransactionType.Sell, note: null, adminCustomerId: null);
            await _cashWallet.CreditSaleAsync(customer, storeId, points, transaction.Id);
            scope.Complete();
        }

        await _eventPublisher.PublishAsync(new GoldSoldEvent(transaction));
        return GoldOperationResult.Ok(transaction);
    }

    public async Task<GoldOperationResult> AdjustAsync(
        int customerId,
        decimal gramsDelta,
        string note,
        int adminCustomerId)
    {
        if (gramsDelta < -9999999999m || gramsDelta > 9999999999m || gramsDelta != GoldPricing.RoundGrams(gramsDelta))
            return GoldOperationResult.Fail("مقدار طلا حداکثر ۸ رقم اعشار دارد و باید در محدوده مجاز باشد.");
        if (gramsDelta == 0)
            return GoldOperationResult.Fail("مقدار تغییر نمی‌تواند صفر باشد.");
        if (string.IsNullOrWhiteSpace(note) || note.Trim().Length > 1000)
            return GoldOperationResult.Fail("دلیل تغییر موجودی را وارد کنید (حداکثر ۱۰۰۰ کاراکتر).");

        // Balance and audit row commit together. A failed ledger insert rolls back the update.
        // Serializable isolation also keeps BalanceAfter consistent for concurrent adjustments.
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var wallet = await GetOrCreateWalletAsync(customerId);
        if (gramsDelta > 0 && await GetBalanceAsync(customerId) > 9999999999m - gramsDelta)
            return GoldOperationResult.Fail("موجودی نهایی از محدوده مجاز بیشتر می‌شود.");
        if (gramsDelta > 0)
            await CreditAtomicAsync(wallet.Id, gramsDelta);
        else if (!await TryDebitAtomicAsync(wallet.Id, -gramsDelta))
            return GoldOperationResult.Fail("موجودی طلای مشتری کمتر از مقدار درخواستی برای کسر است.");

        var transaction = await InsertLedgerRowAsync(
            wallet.Id, customerId, orderId: null, signedGrams: gramsDelta,
            pricePerGram: 0, amount: 0, TransactionType.AdminAdjustment, note.Trim(), adminCustomerId);
        scope.Complete();
        return GoldOperationResult.Ok(transaction);
    }

    #endregion
}
