using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Data;
using Nop.Services.Customers;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;
using ZarIran.Plugin.Misc.GoldPurchase.Enums;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

public class GoldOrderSettlement
{
    private readonly IRepository<GoldOrderWalletPayment> _payments;
    private readonly GoldCashWallet _wallet;
    private readonly ICustomerService _customers;
    private readonly IRepository<GoldTransaction> _goldTransactions;

    public GoldOrderSettlement(IRepository<GoldOrderWalletPayment> payments, GoldCashWallet wallet, ICustomerService customers,
        IRepository<GoldTransaction> goldTransactions)
    {
        _payments = payments;
        _wallet = wallet;
        _customers = customers;
        _goldTransactions = goldTransactions;
    }

    // Called inside the order transaction, after the persisted total is checked.
    public async Task<int> RecordAsync(Order order, Customer customer, int points, decimal amount)
    {
        var salePointsBefore = await _wallet.GetUnspentSalePointsAsync(customer.Id, order.StoreId);
        var entryId = await _wallet.AddRewardPointsHistoryEntryAsync(customer, -points, order.StoreId,
            $"پرداخت خرید طلا #{order.Id}", usedAmount: amount);
        var salePointsAfter = await _wallet.GetUnspentSalePointsAsync(customer.Id, order.StoreId);

        // Keep this debit out of the legacy generic refund flow, which rounds to tens.
        await _payments.InsertAsync(new GoldOrderWalletPayment
        {
            OrderId = order.Id,
            Points = points,
            SalePoints = Math.Clamp(salePointsBefore - salePointsAfter, 0, points)
        }, publishEvent: false);
        return entryId;
    }

    public Task<bool> WasRefundedAsync(int orderId)
        => _payments.Table.AnyAsync(x => x.OrderId == orderId && x.Refunded);

    public async Task RefundIfUnpaidAsync(Order order)
    {
        if (order.PaymentStatus != PaymentStatus.Pending && order.PaymentStatus != PaymentStatus.Voided)
            return;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var payment = await _payments.Table.FirstOrDefaultAsync(x => x.OrderId == order.Id && !x.Refunded);
        if (payment != null && !await _goldTransactions.Table.AnyAsync(x => x.OrderId == order.Id &&
            x.TransactionType == TransactionType.Purchase))
        {
            var customer = await _customers.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null)
                throw new InvalidOperationException($"Customer for gold order {order.Id} could not be found.");

            var changed = await _payments.Table.Where(x => x.Id == payment.Id && !x.Refunded)
                .UpdateAsync(x => new GoldOrderWalletPayment { Refunded = true });
            if (changed > 0)
                await _wallet.RestorePurchaseAsync(customer, order.StoreId, payment.Points, payment.SalePoints, order.Id);
        }

        scope.Complete();
    }
}
