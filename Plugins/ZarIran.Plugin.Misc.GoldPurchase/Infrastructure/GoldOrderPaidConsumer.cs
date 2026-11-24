using Nop.Core.Events;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Logging;
using ZarIran.Plugin.Misc.GoldPurchase.Services;
using System.Threading.Tasks;
using System.Transactions;
using System;
using Nop.Services.Events;

namespace ZarIran.Plugin.Misc.GoldPurchase.Infrastructure;

public class GoldOrderPaidConsumer : IConsumer<OrderPaidEvent>
{
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IGoldWalletService _goldWalletService;
    private readonly ILogger _logger;
    private readonly GoldOrderSettlement _settlement;

    public GoldOrderPaidConsumer(
        IGenericAttributeService genericAttributeService,
        IGoldWalletService goldWalletService,
        ILogger logger,
        GoldOrderSettlement settlement)
    {
        _genericAttributeService = genericAttributeService;
        _goldWalletService = goldWalletService;
        _logger = logger;
        _settlement = settlement;
    }

    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        var order = eventMessage.Order;
        if (order == null || order.Deleted || order.OrderStatus == OrderStatus.Cancelled)
            return;

        var amount = await _genericAttributeService.GetAttributeAsync<decimal?>(
            order, GoldPurchaseDefaults.OrderAmountAttribute);

        var pricePerGram = await _genericAttributeService.GetAttributeAsync<decimal?>(
            order, GoldPurchaseDefaults.OrderGoldPriceAttribute);

        var grams = await _genericAttributeService.GetAttributeAsync<decimal?>(
            order, GoldPurchaseDefaults.OrderGoldWeightAttribute);

        if (!amount.HasValue || !pricePerGram.HasValue || !grams.HasValue || grams.Value <= 0)
            return;

        try
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            if (!await _settlement.WasRefundedAsync(order.Id))
            {
                await _goldWalletService.CreditPurchaseAsync(
                    order.CustomerId,
                    order.Id,
                    amount.Value,
                    pricePerGram.Value,
                    grams.Value);
            }
            scope.Complete();
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync(
                $"Failed to credit gold wallet for order #{order.Id}.",
                ex);
            throw;
        }
    }
}
