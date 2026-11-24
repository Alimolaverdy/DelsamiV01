using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

// Use nopCommerce's ledger without the wallet plugin's rounding to tens.
public class GoldCashWallet : RewardPointService
{
    private const string SalePrefix = "اعتبار فروش طلا";
    private readonly RewardPointsSettings _settings;

    public GoldCashWallet(IDateTimeHelper dateTimeHelper, ILocalizationService localization,
        IRepository<RewardPointsHistory> history, RewardPointsSettings settings)
        : base(dateTimeHelper, localization, history, settings)
    {
        _settings = settings;
    }

    public decimal ToAmount(int points) => _settings.Enabled && _settings.ExchangeRate > 0
        ? Math.Max(0, points) * _settings.ExchangeRate
        : 0m;

    public int ToPoints(decimal amount)
    {
        if (!_settings.Enabled || _settings.ExchangeRate <= 0 || amount <= 0 ||
            amount > int.MaxValue * _settings.ExchangeRate)
            return 0;

        var points = decimal.ToInt32(decimal.Floor(amount / _settings.ExchangeRate));
        return ToAmount(points) == amount ? points : 0;
    }

    public int PointsUpTo(decimal amount, int availablePoints)
    {
        if (!_settings.Enabled || _settings.ExchangeRate <= 0 || availablePoints <= 0 || amount <= 0)
            return 0;

        return ToAmount(availablePoints) <= amount
            ? availablePoints
            : decimal.ToInt32(decimal.Floor(amount / _settings.ExchangeRate));
    }

    private async Task<IQueryable<RewardPointsHistory>> GetSaleEntriesAsync(int customerId, int storeId)
    {
        return (await GetRewardPointsQueryAsync(customerId, storeId))
            .Where(entry => entry.ValidPoints > 0 && entry.Message.StartsWith(SalePrefix));
    }

    public async Task<int> GetUnspentSalePointsAsync(int customerId, int storeId)
    {
        return (await GetSaleEntriesAsync(customerId, storeId)).Sum(entry => entry.ValidPoints) ?? 0;
    }

    public Task<int> CreditSaleAsync(Customer customer, int storeId, int points, int saleId)
        => AddRewardPointsHistoryEntryAsync(customer, points, storeId, $"{SalePrefix} #{saleId}");

    // Reserve a payout by consuming only gold-sale credits. Normal purchases still use nopCommerce FIFO.
    public async Task<bool> ReserveWithdrawalAsync(Customer customer, int storeId, int points, int withdrawalId)
    {
        var balance = await GetRewardPointsBalanceAsync(customer.Id, storeId);
        if (points <= 0 || points > balance)
            return false;

        var entries = (await GetSaleEntriesAsync(customer.Id, storeId))
            .OrderBy(entry => entry.CreatedOnUtc).ThenBy(entry => entry.Id).ToList();
        if (points > entries.Sum(entry => entry.ValidPoints))
            return false;

        await InsertRewardPointsHistoryEntryAsync(new RewardPointsHistory
        {
            CustomerId = customer.Id,
            StoreId = storeId,
            Points = -points,
            PointsBalance = balance - points,
            UsedAmount = ToAmount(points),
            Message = $"درخواست برداشت وجه فروش طلا #{withdrawalId}",
            CreatedOnUtc = DateTime.UtcNow
        });

        var remaining = points;
        foreach (var entry in entries)
        {
            var taken = Math.Min(remaining, entry.ValidPoints.Value);
            entry.ValidPoints -= taken;
            await UpdateRewardPointsHistoryEntryAsync(entry);
            remaining -= taken;
            if (remaining == 0)
                break;
        }

        return true;
    }

    public async Task RestorePurchaseAsync(Customer customer, int storeId, int points, int salePoints, int orderId)
    {
        if (points > salePoints)
            await AddRewardPointsHistoryEntryAsync(customer, points - salePoints, storeId, $"بازگشت پرداخت خرید طلا #{orderId}");
        if (salePoints > 0)
            await AddRewardPointsHistoryEntryAsync(customer, salePoints, storeId, $"{SalePrefix} · بازگشت خرید #{orderId}");
    }
}
