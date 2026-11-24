using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Events;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

namespace ZarIran.Plugin.Misc.GoldPurchase.Infrastructure;

public class GoldOrderCancelledConsumer : IConsumer<EntityUpdatedEvent<Order>>, IConsumer<EntityDeletedEvent<Order>>
{
    private readonly GoldOrderSettlement _settlement;

    public GoldOrderCancelledConsumer(GoldOrderSettlement settlement) => _settlement = settlement;

    public Task HandleEventAsync(EntityUpdatedEvent<Order> message)
        => message.Entity.OrderStatus == OrderStatus.Cancelled || message.Entity.Deleted
            ? _settlement.RefundIfUnpaidAsync(message.Entity)
            : Task.CompletedTask;

    public Task HandleEventAsync(EntityDeletedEvent<Order> message)
        => _settlement.RefundIfUnpaidAsync(message.Entity);
}
