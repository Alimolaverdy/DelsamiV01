using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ZarIran.Plugin.Payments.TorobPay.Infrastructures.Permissions;
using ZarIran.Plugin.Payments.TorobPay.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ZarIran.Plugin.Payments.TorobPay.Services.Dtos.PaymentDto;
using IPaymentService = ZarIran.Plugin.Payment.Core.Services.IPaymentService;
using ZarIran.Plugin.Payment.Core.Enums;
using ZarIran.Plugin.Payment.Core.Domain;

namespace Nop.Plugin.Payments.TorobPay
{
    public class TorobPayPaymentPlugin : BasePlugin, IPaymentMethod, IPlugin, IAdminMenuPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILogger _logger;
        private readonly TorobPaySettings _settings;
        private readonly ITorobPayApiService _apiService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public TorobPayPaymentPlugin(
                TorobPaySettings settings,
                ISettingService settingService,
                ITorobPayApiService apiService,
                ILogger logger,
                IOrderTotalCalculationService orderTotalCalculationService,
                ILocalizationService localizationService,
                IWebHelper webHelper,
                IOrderService orderService,
                IProductService productService,
                ICategoryService categoryService,
                IPermissionService permissionService,
                ICustomerService customerService,
                IStoreContext storeContext,
                IHttpContextAccessor httpContextAccessor,
                IPaymentService paymentService)
        {
            _settings = settings;
            _settingService = settingService;
            _apiService = apiService;
            _logger = logger;
            _orderTotalCalculationService = orderTotalCalculationService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _orderService = orderService;
            _productService = productService;
            _categoryService = categoryService;
            _permissionService = permissionService;
            _customerService = customerService;
            _storeContext = storeContext;
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => true;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => true;

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest) => Task.FromResult(new CancelRecurringPaymentResult { Errors = new List<string> { "Recurring not supported" } });

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest) => Task.FromResult(new CapturePaymentResult { Errors = new List<string> { "Not supported" } });

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart) => Task.FromResult(decimal.Zero);

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form) => Task.FromResult(new ProcessPaymentRequest());

        public async Task<string> GetPaymentMethodDescriptionAsync() => await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.TorobPay.Description");

        public Type GetPublicViewComponent() => null;

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest) => Task.FromResult(new ProcessPaymentResult { Errors = new List<string> { "Recurring not supported" } });

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest) => Task.FromResult(new VoidPaymentResult { Errors = new List<string> { "Not supported" } });

        public Task<bool> CanRePostProcessPaymentAsync(Order order) => Task.FromResult(true);



        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentException(nameof(form));

            //try to get errors
            if (form.TryGetValue("Errors", out StringValues errorsString) && !StringValues.IsNullOrEmpty(errorsString))
            {
                var errorList = new List<string> { errorsString.ToString() };
                return Task.FromResult<IList<string>>(errorList);
            }

            return Task.FromResult<IList<string>>(new List<string>());
        }



        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Pending
            };

            return await Task.FromResult(result);
        }


        public async Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            try
            {
                if (cart == null || cart.Count == 0)
                    return true;

                var total = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false);
                var shoppingCartTotal = total.shoppingCartTotal;
                if (!shoppingCartTotal.HasValue)
                    return true;


                if (shoppingCartTotal <= _settings.MinimumAllowedAmount || shoppingCartTotal >= _settings.MaximumAllowedAmount)
                    return true;

                var eligible = await _apiService.CheckEligibilityAsync(shoppingCartTotal.Value);
                return !eligible.eligible;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("TorobPay.HidePaymentMethodAsync error", ex);
                return true;
            }
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = postProcessPaymentRequest?.Order;
            var customerMobileNo = _customerService.GetCustomerByIdAsync(order.CustomerId).Result.Username;
            if (order == null)
                return;


            var storeUrl = _webHelper.GetStoreLocation();
            if (!storeUrl.EndsWith("/"))
                storeUrl += "/";

            var errorPageUrl = $"{storeUrl}Checkout/PaymentError?orderId={order.Id}";

            try
            {
                var payment = new Payment
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    SourceType = SourceType.Order,
                    Gateway = PaymentGateway.Torob,
                    Amount = order.OrderTotal,
                    CreatedDateTime = DateTime.UtcNow,
                    Status = ZarIran.Plugin.Payment.Core.Enums.PaymentStatus.Initial,
                    TeminalNo = _settings.ClientId,
                    PaymentType = PaymentType.CPG
                };
                await _paymentService.InsertAsync(payment);
                var amount = order.OrderTotal;
                var cartItems = await BuildCartItemsAsync(order.Id, amount);

                var cartList = new List<Cart>
                    {
                        new Cart
                        {
                            cartId = order.Id.ToString(),
                            totalAmount = amount,
                            cartItems = cartItems
                        }
                    };

                var currentStoreUrl = _webHelper.GetStoreLocation();
                if (currentStoreUrl.EndsWith("/"))
                    currentStoreUrl = currentStoreUrl.Substring(0, currentStoreUrl.Length - 1);

                var req = new PaymentTokenRequest
                {
                    amount = amount,
                    paymentMethodTypeDto = "ONLINE_CREDIT",
                    returnURL = $"{currentStoreUrl}/TorobPayPayment/Verify?paymentId={payment.Id}",
                    transactionId = payment.Id.ToString(),
                    mobile = customerMobileNo,
                    cartList = cartList
                };

                var tokenResp = await _apiService.CreatePaymentTokenAsync(req);
                payment.UrlId = tokenResp.paymentToken;
                order.AuthorizationTransactionCode = tokenResp.paymentToken;
                payment.Status = ZarIran.Plugin.Payment.Core.Enums.PaymentStatus.InProgress;
                await _paymentService.UpdateAsync(payment);
                await _orderService.UpdateOrderAsync(order);
                _httpContextAccessor.HttpContext.Response.Redirect(tokenResp.paymentPageUrl);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("TorobPay.PostProcessPaymentAsync error", ex);
                _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
            }
        }

        private async Task<List<CartItem>> BuildCartItemsAsync(int orderId, decimal orderAmount)
        {
            var cartItems = new List<CartItem>();
            var orderItems = await _orderService.GetOrderItemsAsync(orderId);

            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                cartItems.Add(new CartItem
                {
                    id = item.Id.ToString(),
                    name = product?.Name,
                    count = item.Quantity,
                    amount = orderAmount
                });
            }

            return cartItems;
        }

        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();

            try
            {
                var order = refundPaymentRequest.Order;
                var paymentToken = order.AuthorizationTransactionCode;

                if (string.IsNullOrEmpty(paymentToken))
                {
                    result.AddError("کد تراکنش ذخیره نشده است (paymentToken).");
                    return result;
                }

                if (order.PaymentStatus != Core.Domain.Payments.PaymentStatus.Paid)
                {
                    result.AddError("سفارش در وضعیت پرداخت‌شده نیست.");
                    return result;
                }

                await _apiService.CancelAsync(paymentToken);

                await _logger.InformationAsync($"TorobPay: لغو تراکنش موفق. Token={paymentToken}");

                result.NewPaymentStatus = Core.Domain.Payments.PaymentStatus.Refunded;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("TorobPay RefundAsync error", ex);
                result.AddError("خطا در اجرای لغو پرداخت: " + ex.Message);
            }

            return result;
        }



        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(TorobPayPermissions.ManageTorobPay))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var torobPaySystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.TorobPay.SystemName");
            var torobPayName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.TorobPay.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.TorobPay.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.TorobPay.Configuration.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var registerCustomerChildNodes = new List<SiteMapNode>
            {
                 new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "TorobPay",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }
                }
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = torobPayName,
                SystemName = torobPaySystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = registerCustomerChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Payments.TorobPay.SystemName"] = "TorobPay",
                ["ZarIran.Plugin.Payments.TorobPay.Name"] = "ZarIran TorobPay",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.SystemName"] = "TorobPayConfiguration",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.ClientId"] = "ClientId",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.ClientSecret"] = "ClientSecret",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.Username"] = "Username",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.Password"] = "Password",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.ReturnUrl"] = "ReturnUrl",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.BaseUrl"] = "BaseUrl",
                ["ZarIran.Plugin.Payments.TorobPay.Configuration.TokenCacheMinutes"] = "TokenCacheMinutes",
                ["ZarIran.Plugin.Payments.TorobPay.Description"] = "Torob Pay",
                ["ZarIran.Plugin.Payments.TorobPay.MinimumAllowedAmount"] = "MinimumAllowedAmount",
                ["ZarIran.Plugin.Payments.TorobPay.MaximumAllowedAmount"] = "MaximumAllowedAmount",
                ["ZarIran.Plugin.Payments.TorobPay.TimeoutSeconds"] = "TimeoutSeconds",
                ["ZarIran.Plugin.Payments.TorobPay.TimeoutSeconds"] = "TimeoutSeconds"
            });

            await _permissionService.InstallPermissionsAsync(new TorobPayPermissions());

            if (_settingService.LoadSettingAsync<TorobPaySettings>() == null)
            {
                var setting = new TorobPaySettings { ClientId = "92129000", ClientSecret = "mF1Ur0I7FmByxp7gQA7RTQckdUMbzNOLtQVqvbRjA9g1EpvGBWlrpL1PVCYriBfz", Username = "ZarIran.com", Password = "eIMEL5CIs9R0uiwV", BaseUrl = "https://cpg.torobpay.com", TokenCacheMinutes = 55, MinimumAllowedAmount = 200000, MaximumAllowedAmount = 1000000000, TimeoutSeconds = 5 };
                await _settingService.SaveSettingAsync(setting);
            }
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }


    }
}
