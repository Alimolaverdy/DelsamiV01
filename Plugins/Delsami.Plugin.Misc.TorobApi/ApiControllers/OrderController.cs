using Microsoft.AspNetCore.Mvc;
using Delsami.Plugin.Misc.Core.Bases;
using Delsami.Plugin.Misc.ExtendedOrder.Services;
using Delsami.Plugin.Misc.TorobApi.Filters;
using Delsami.Plugin.Misc.TorobApi.Models;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.TorobApi.ApiControllers
{
    [ApiController]
    [Route("api/torob/v1/orders")]
    public class OrderController : BaseApiController
    {
        #region Fields

        private readonly TorobApiSettings _torobApiSettings;
        private readonly ILogger _logger;
        private readonly IExtendedOrderService _extendedOrderService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public OrderController(TorobApiSettings torobApiSettings,
                                ILogger logger,
                                IExtendedOrderService extendedOrderService,
                                IUrlRecordService urlRecordService,
                                IStoreContext storeContext)
        {
            _torobApiSettings = torobApiSettings;
            _logger = logger;
            _extendedOrderService = extendedOrderService;
            _urlRecordService = urlRecordService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        [ServiceFilter(typeof(TorobAuthenticationFilter))]
        [HttpGet]
        public async Task<IActionResult> GetOrders(DateTime purchase_timestamp_gt, int limit)
        {
            if (_torobApiSettings.LogEnabledOrderRequest)
                await _logger.InformationAsync($"Torob Api Get Order Request: purchase_timestamp_gt:{purchase_timestamp_gt} limit:{limit}");

            if (!_torobApiSettings.IsActive)
            {
                if (_torobApiSettings.LogEnabledOrderRequest)
                    await _logger.InformationAsync($"Torob Api Get Order Request: unauthorized response");

                return StatusCode(403, new
                {
                    success = false
                });
            }

            if (limit <= 0 || limit > 1000)
            {
                if (_torobApiSettings.LogEnabledOrderRequest)
                    await _logger.InformationAsync($"Torob Api Get Order Request: bad request response");

                return BadRequest(new
                {
                    success = false,
                    message = "limit must be between 1 and 1000.",
                    data = Array.Empty<object>()
                });
            }


            try
            {
                var orders = await _extendedOrderService.GetOrdersAsync(purchase_timestamp_gt, limit);

                var productsOrderItems = await _extendedOrderService.GetProductsOrderItemsByOrderIdsAsync(orders.Select(s => s.Id).ToList());

                List<TorobOrderDto> torobOrders = new();

                var storeUrl = (await _storeContext.GetCurrentStoreAsync()).Url.TrimEnd('/');

                foreach (var order in orders)
                {
                    var torobOrder = new TorobOrderDto
                    {
                        purchase_timestamp = order.PaidDateUtc.Value.ToLocalTime(),
                        last_updated_timestamp = order.PaidDateUtc.Value.ToLocalTime(),
                        torob_clid = order.CardName,
                        psp = GetPaymentName(order.PaymentMethodSystemName),
                        status = GetTorobOrderStatus(order.OrderStatus),
                        order_value = (int)order.OrderTotal,
                        shipping_amount = (int)order.OrderShippingInclTax,
                    };

                    var torobOrderProducts = (await Task.WhenAll(productsOrderItems.OrderItems.Where(w => w.OrderId == order.Id).Select(async s =>
                    {
                        var groupedProduct = productsOrderItems.ProductsDic[s.ProductId];

                        if (groupedProduct is null)
                            return null;

                        var torobOrderProduct = new TorobOrderProductDto
                        {
                            product_price = (int)s.UnitPriceExclTax,
                            product_url = $"{storeUrl}/{await _urlRecordService.GetSeNameAsync(groupedProduct)}",
                            quantity = s.Quantity
                        };

                        return torobOrderProduct;

                    }))).ToList();

                    torobOrder.products = torobOrderProducts.Where(w => w is not null).ToList();

                    torobOrders.Add(torobOrder);
                }

                if (_torobApiSettings.LogEnabledOrderRequest)
                    await _logger.InformationAsync($"Torob Api Get Order Request: {Newtonsoft.Json.JsonConvert.SerializeObject(orders)}");

                return Ok(new
                {
                    success = true,
                    data = torobOrders.OrderBy(o => o.purchase_timestamp)
                });

            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Torob Api Get Order Request: failed due to: {ex.Message}", ex);

                return StatusCode(500, new
                {
                    success = false,
                    data = Array.Empty<object>()
                });
            }
        }

        private string GetPaymentName(string paymentSytemName)
        {
            if (string.IsNullOrWhiteSpace(paymentSytemName))
                return string.Empty;

            if (paymentSytemName == "Payments.CheckMoneyOrder")
                return "CashOnDelivery";

            var result = paymentSytemName.Contains('.')
                        ? paymentSytemName[(paymentSytemName.LastIndexOf('.') + 1)..]
                        : paymentSytemName;


            return result;
        }
        private string GetTorobOrderStatus(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Cancelled:
                    return "cancelled";
                case OrderStatus.Processing:
                    return "completed";
                case OrderStatus.Complete:
                    return "completed";
                case OrderStatus.Pending:
                    return "on-hold";
                default:
                    return string.Empty;
            }
        }
    }


    #endregion
}
