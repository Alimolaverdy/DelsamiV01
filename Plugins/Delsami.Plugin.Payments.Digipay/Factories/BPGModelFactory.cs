using Delsami.Plugin.Payment.Core.Domain;
using Delsami.Plugin.Payments.Digipay.Models.DigiPays;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.Digipay.Factories
{
    public class BPGModelFactory : IBPGModelFactory
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public BPGModelFactory(IWebHelper webHelper, IOrderService orderService, ICustomerService customerService)
        {
            _webHelper = webHelper;
            _orderService = orderService;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public async Task<CreateTicketModel> PrepareCreateTicketModelAsync(Order order, int paymentId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            var BasketDetailItems = orderItems.Select(oi => new BasketDetailItemModel
            {
                count = oi.Quantity,
                productCode = oi.ProductId.ToString(),
                brand = "brand",
                productType = 1
            }).ToList();

            var basketDetail = new BasketDetailModel
            {
                basketId = order.Id.ToString(),
                items = BasketDetailItems
            };
            var currentStoreUrl = _webHelper.GetStoreLocation();
            currentStoreUrl = currentStoreUrl.EndsWith("/") ? currentStoreUrl.Substring(0, currentStoreUrl.Length - 1) : currentStoreUrl;

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            var createTicketModel = new CreateTicketModel()
            {
                amount = Convert.ToInt64(order.OrderTotal),
                callbackUrl = currentStoreUrl + $"/DigiPay/Verify?paymentId={paymentId}",
                cellNumber = customer.Username,
                providerId = paymentId.ToString(),
                basketDetailsDto = basketDetail
            };

            return createTicketModel;
        }

        public DeliverRequestModel PrepareDeliverRequest(int orderId, string trackingCode, List<string> productNames)
        {
            var preparedProducts = new List<string>();

            foreach (var productName in productNames)
            {
                preparedProducts.Add("خودرو و موتور سیکلت");
                preparedProducts.Add("لوازم جانبی خودرو");
                preparedProducts.Add(productName);
            }

            return new DeliverRequestModel
            {
                deliveryDate = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                invoiceNumber = orderId.ToString(),
                trackingCode = trackingCode,
                products = preparedProducts
            };
        }

        public VerifyRequestModel PrepareVerifyRequest(int paymentId, string trackingCode)
        {
            return new VerifyRequestModel { providerId = paymentId.ToString(), trackingCode = trackingCode };
        }

        public RefundRequestModel PrepareRefundRequest(Payment.Core.Domain.Payment payment, string trackingCode)
        {
            return new RefundRequestModel
            {
                providerId = payment.Id.ToString(),
                saleTrackingCode = trackingCode,
                amount = ((int)payment.Amount).ToString()
            };
        }

        #endregion
    }
}
