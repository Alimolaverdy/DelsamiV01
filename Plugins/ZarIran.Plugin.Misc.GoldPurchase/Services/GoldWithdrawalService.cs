using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using ZarIran.Plugin.Misc.ExtendedCustomer.Services;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

public class GoldWithdrawalService
{
    private readonly IRepository<GoldWithdrawal> _requests;
    private readonly ICustomerShebaNumberService _shebas;
    private readonly ICustomerService _customers;
    private readonly GoldCashWallet _wallet;

    public GoldWithdrawalService(IRepository<GoldWithdrawal> requests, ICustomerShebaNumberService shebas,
        ICustomerService customers, GoldCashWallet wallet)
    {
        _requests = requests;
        _shebas = shebas;
        _customers = customers;
        _wallet = wallet;
    }

    public async Task<decimal> GetAvailableAsync(int customerId, int storeId)
    {
        var balance = await _wallet.GetRewardPointsBalanceAsync(customerId, storeId);
        var salePoints = await _wallet.GetUnspentSalePointsAsync(customerId, storeId);
        return decimal.Floor(_wallet.ToAmount(Math.Min(balance, salePoints)));
    }

    public async Task<string> RequestAsync(Customer customer, int storeId, Guid requestKey, decimal amount, int shebaId)
    {
        var points = _wallet.ToPoints(amount);
        if (requestKey == Guid.Empty || points <= 0 || amount > long.MaxValue || decimal.Truncate(amount) != amount)
            return "مبلغ برداشت را به تومان و به صورت عدد صحیح وارد کنید.";

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var previous = await _requests.Table.FirstOrDefaultAsync(x => x.RequestKey == requestKey);
        if (previous != null)
        {
            scope.Complete();
            return previous.CustomerId == customer.Id && previous.StoreId == storeId
                ? null
                : "درخواست معتبر نیست. صفحه را دوباره باز کنید.";
        }

        var sheba = await _shebas.GetCustomerShebaNumberAsync(shebaId);
        var normalized = sheba?.CustomerId == customer.Id ? GoldSheba.Normalize(sheba.Number) : null;
        if (normalized == null)
            return "یک شماره شبای معتبر از حساب خود انتخاب کنید.";

        var request = new GoldWithdrawal
        {
            RequestKey = requestKey,
            CustomerId = customer.Id,
            StoreId = storeId,
            CustomerName = await _customers.GetCustomerFullNameAsync(customer) ?? string.Empty,
            Phone = customer.Phone ?? GetMobileUsername(customer.Username),
            Sheba = normalized,
            Amount = decimal.ToInt64(amount),
            CreatedOnUtc = DateTime.UtcNow
        };
        await _requests.InsertAsync(request, publishEvent: false);
        if (!await _wallet.ReserveWithdrawalAsync(customer, storeId, points, request.Id))
            return "اعتبار قابل برداشت شما برای این مبلغ کافی نیست.";

        scope.Complete();
        return null;
    }

    private static string GetMobileUsername(string username)
    {
        if (!string.IsNullOrEmpty(username) && username.Length == 11 &&
            username.StartsWith("09", StringComparison.Ordinal) && username.All(char.IsDigit))
            return username;
        return string.Empty;
    }

    public Task<IPagedList<GoldWithdrawal>> ListAsync(bool completed, int page)
        => _requests.GetAllPagedAsync(q => q.Where(x => x.CompletedOnUtc.HasValue == completed)
            .OrderByDescending(x => x.Id), pageIndex: page - 1, pageSize: 25);

    public Task<GoldWithdrawal> FindAsync(int id) => _requests.GetByIdAsync(id);

    public async Task CompleteAsync(int id, int adminId)
    {
        var now = DateTime.UtcNow;
        await _requests.Table.Where(x => x.Id == id && !x.CompletedOnUtc.HasValue)
            .UpdateAsync(x => new GoldWithdrawal { CompletedOnUtc = now, CompletedByCustomerId = adminId });
    }
}
