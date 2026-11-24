using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Orders;
using NopShop.Plugin.Misc.Wallet.Domain;

namespace NopShop.Plugin.Misc.Wallet.Infrastructure.Cache
{
    /// <summary>
    /// Represents extended reward points program event consumer
    /// </summary>
    public partial class EventConsumer :
        IConsumer<OrderPaidEvent>
        //IConsumer<OrderRefundedEvent>
    {
        #region Fields
        private readonly WalletSettings _walletSettings;
        private readonly IOrderService _orderService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public EventConsumer(WalletSettings walletSettings,
            IOrderService orderService,
            IRewardPointService rewardPointService,
            ICustomerService customerService,
            IStoreContext storeContext)
        {
            _walletSettings = walletSettings;
            _orderService = orderService;
            _rewardPointService = rewardPointService;
            _customerService = customerService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
            var orderId = eventMessage.Order.Id;
            var items = await _orderService.GetOrderItemsAsync(orderId);
            foreach (var item in items)
            {
                var product = await _orderService.GetProductByOrderItemIdAsync(item.Id);
                if (product.Id == _walletSettings.WalletProductId)
                    await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, Convert.ToInt32(item.PriceInclTax), (await _storeContext.GetCurrentStoreAsync()).Id, "افزایش موجودی کیف پول", eventMessage.Order);
            }
        }
        // Commented Bec Of ZarIran.Payments.TorobPay
        //public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
        //{
        //    var customer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
        //    await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, Convert.ToInt32(eventMessage.Amount), (await _storeContext.GetCurrentStoreAsync()).Id, "افزایش موجودی کیف پول برای مرجوعی سفارش " + $"#{eventMessage.Order.Id}", eventMessage.Order);
        //}

        #endregion
    }
}
