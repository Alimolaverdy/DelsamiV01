using Delsami.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.ScheduleTasks
{
    public class CompleteProcessingOrderScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly IExtendedOrderService _extendedOrderService;
        private readonly IOrderService _orderService;
        private readonly ExtendedOrderSettings _extendedOrderSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor

        public CompleteProcessingOrderScheduleTask(
                                       ExtendedOrderSettings extendedOrderSettings,
                                       ICustomerActivityService customerActivityService,
                                       ILocalizationService localizationService,
                                       IEventPublisher eventPublisher,
                                       IExtendedOrderService extendedOrderService,
                                       IOrderService orderService)
        {
            _extendedOrderSettings = extendedOrderSettings;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _extendedOrderService = extendedOrderService;
            _orderService = orderService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var orderStatus = new List<int>() { (int)OrderStatus.Processing };

            if (_extendedOrderSettings.OrderCompleteAsDays == 0)
                return;

            if (string.IsNullOrWhiteSpace(_extendedOrderSettings.SelectedShippingMethods))
                return;

            var toDateTimeUtc = DateTime.UtcNow.AddDays(-_extendedOrderSettings.OrderCompleteAsDays);
            var shippingMethodIds = _extendedOrderSettings.SelectedShippingMethods.Split(",").Select(int.Parse).ToList();

            if (!shippingMethodIds.Any())
                return;

            var processingOrders = await _extendedOrderService.SearchOrdersAsync(ShippingMethodIds: shippingMethodIds, osIds: orderStatus, createdToUtc: toDateTimeUtc);

            foreach (var processingOrder in processingOrders)
            {
                if (processingOrder.PaymentStatus != Nop.Core.Domain.Payments.PaymentStatus.Paid)
                    continue;

                var prevOrderStatus = processingOrder.OrderStatus;

                processingOrder.ShippingStatusId = (int)ShippingStatus.Delivered;
                processingOrder.OrderStatusId = (int)OrderStatus.Complete;

                await _orderService.UpdateOrderAsync(processingOrder);

                await _eventPublisher.PublishAsync(new OrderStatusChangedEvent(processingOrder, OrderStatus.Processing));

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = processingOrder.Id,
                    Note = $"Order status has been edited automatically by schedule. New status: {await _localizationService.GetLocalizedEnumAsync(processingOrder.OrderStatus)}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });


                await _customerActivityService.InsertActivityAsync("EditOrder",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditOrder"), processingOrder.CustomOrderNumber), processingOrder);
            }
        }

        #endregion
    }
}
