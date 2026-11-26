using Delsami.Plugin.Misc.Core;
using Delsami.Plugin.Misc.Core.Services;
using Delsami.Plugin.Payment.Core.Services;
using Delsami.Plugin.Payments.Barook.Models.Barook;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.Barook.Services
{
    public class BarookService : IBarookService
	{
		#region Fields

		private readonly BarookSettings _BarookSettings;
		private readonly ILogger _logger;
		private readonly IWebHelper _webHelper;
		private readonly IPaymentApiCaller _paymentApiCaller;
		private readonly CoreSettings _coreSettings;
		private readonly IDelsamiCustomerService  _DelsamiCustomerService;
		private readonly IOrderService _orderService;
		private readonly IProductService  _productService;

		#endregion

		#region Ctor

		public BarookService(BarookSettings BarookSettings,
							ILogger logger,
							IPaymentApiCaller paymentApiCaller,
							IWebHelper webHelper,
							CoreSettings coreSettings,
                            IDelsamiCustomerService DelsamiCustomerService,
							IOrderService orderService,
							IProductService productService)
		{
			_BarookSettings = BarookSettings;
			_logger = logger;
			_webHelper = webHelper;
			_paymentApiCaller = paymentApiCaller;
			_coreSettings = coreSettings;
            _DelsamiCustomerService = DelsamiCustomerService;
			_orderService = orderService;
			_productService = productService;
		}

		#endregion

		#region Methods
		public async Task<(bool CallIsSuccess, StartPaymentResponse tokenResult)> GetToken(Payment.Core.Domain.Payment payment, Customer customer)
		{

			try
			{
				var url = _BarookSettings.BaseUrl + BarookPaymentEndpoints.GetToken;

				var currentStoreUrl = _webHelper.GetStoreLocation();
				if (currentStoreUrl.EndsWith("/"))
					currentStoreUrl = currentStoreUrl.Substring(0, currentStoreUrl.Length - 1);

				var nationalCode = _DelsamiCustomerService.GetNationalCode(customer.CustomCustomerAttributesXML);

				var BasketDetailItems = await PrepareOrderItems(payment.OrderId);

				var tokenRequest = new StartPaymentRequest
				{
					BusinessServiceSlug = _BarookSettings.businessServiceSlug,
					ExternalCode = payment.Id.ToString(),
					OwnerMobile = customer.Username,
					OwnerName = customer.FirstName + " " + customer.LastName,
					OwnerNationalCode = nationalCode,
					paymentDailyCount = _BarookSettings.paymentDailyCount,
					TerminalCode = _BarookSettings.TerminalCode,
					Password = _BarookSettings.TerminalPassword,
					TotalAmount = (long)payment.Amount,
					RedirectUrl = currentStoreUrl + $"/Barook/VerifyGet?paymentId={payment.Id}",
					SaleItemDto = BasketDetailItems,

				};

				var result = await _paymentApiCaller.PostAsync<StartPaymentRequest, StartPaymentResponse>(url, tokenRequest);

				return (result.IsSuccess, result.Data);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message, ex);
				return (false, null);
			}
		}



		public async Task<(bool CallIsSuccess, VerifyPaymentResponse VerifyResponse)> VerifyAsync(VerifyPaymentRequest verifyRequest)
		{
			try
			{
				var url = _BarookSettings.BaseUrl + BarookPaymentEndpoints.Verify;

				var result = await _paymentApiCaller.PostAsync<VerifyPaymentRequest, VerifyPaymentResponse>(url, verifyRequest);

				return (result.IsSuccess, result.Data);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message, ex);
				return (false, null);
			}
		}



		private async Task<List<SaleItemDto>> PrepareOrderItems(int orderId)
		{
			var orderItems = await _orderService.GetOrderItemsAsync(orderId);
			var products =(await _productService.GetProductsByIdsAsync(orderItems.Select(a => a.ProductId).ToArray())).ToDictionary(a=>a.Id,a=>a);

			return orderItems.Select(oi => new SaleItemDto
			{
				Name = products[oi.ProductId].Name,
				SaleCount = oi.Quantity,
				Amount = (long)oi.UnitPriceInclTax,
				TotalAmount= (long)oi.PriceInclTax,
				UnitType = "عدد",
				Sku=products[oi.ProductId].Sku,

			}).ToList();
		}
		#endregion

	}
}
