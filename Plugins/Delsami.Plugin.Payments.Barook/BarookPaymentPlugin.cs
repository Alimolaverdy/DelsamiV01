using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZarIran.Plugin.Payment.Core.Enums;
using ZarIran.Plugin.Payments.Barook.Services;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IPaymentService = ZarIran.Plugin.Payment.Core.Services.IPaymentService;

namespace ZarIran.Plugin.Payments.Barook
{
    public class BarookPaymentPlugin : BasePlugin, IPaymentMethod, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingsService;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly IBarookService _BarookService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly BarookSettings _BarookSettings;
        private readonly IPermissionService _permissionService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public BarookPaymentPlugin(ILocalizationService localizationService,
                                          ISettingService settingsService,
                                          IHttpContextAccessor httpContextAccessor,
                                          IWebHelper webHelper,
										  ICustomerService customerService,
                                          IBarookService BarookService,
                                          IOrderService orderService,
                                          IPaymentService paymentService,
                                          BarookSettings BarookSettings,
                                          IPermissionService permissionService,
                                          ILogger logger)
        {
            _localizationService = localizationService;
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
            _BarookService = BarookService;
            _customerService = customerService;
            _orderService = orderService;
            _paymentService = paymentService;
            _BarookSettings = BarookSettings;
            _permissionService = permissionService;
            _logger = logger;
        }

        #endregion

        #region Properties

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public async Task<string> GetPaymentMethodDescriptionAsync() => await _localizationService.GetResourceAsync("ZarIran.Payments.Barook.PaymentMethodDescription");

        #endregion

        #region Methods

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = postProcessPaymentRequest.Order;
            if (order == null)
                throw new ArgumentNullException(nameof(order));


            var storeUrl = _webHelper.GetStoreLocation();
            if (!storeUrl.EndsWith("/"))
                storeUrl += "/";

            var errorPageUrl = $"{storeUrl}Checkout/PaymentError";

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var payment = new Payment.Core.Domain.Payment
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    SourceType = SourceType.Order,
                    Gateway = PaymentGateway.Barook,
                    Amount = order.OrderTotal,
                    CreatedDateTime = DateTime.UtcNow,
                    Status = PaymentStatus.Initial,
                    PaymentType = PaymentType.CPG,
                    TeminalNo = "",

                };
                await _paymentService.InsertAsync(payment);

                var tokenResponse = await _BarookService.GetToken(payment, customer);

                await _logger.InformationAsync($"Barook Get Token response in start payment:{JsonConvert.SerializeObject(tokenResponse)}");

                bool paymentProcessingIsOk = tokenResponse.CallIsSuccess && tokenResponse.tokenResult != null && tokenResponse.tokenResult.Token != null;

                if (paymentProcessingIsOk)
                {

                    var orderNote = new OrderNote
                    {
                        OrderId = order.Id,
                        CreatedOnUtc = DateTime.UtcNow,
                        DisplayToCustomer = false,
                        Note = $"Barook Ticket: {tokenResponse.tokenResult.Token}",
                    };

                    order.AuthorizationTransactionCode = tokenResponse.tokenResult.Token;
                    payment.Status = PaymentStatus.InProgress;

                    await _orderService.InsertOrderNoteAsync(orderNote);
                    await _orderService.UpdateOrderAsync(order);
                    await _paymentService.UpdateAsync(payment);

                    var paymenPage = _BarookSettings.BaseUrl + BarookPaymentEndpoints.Purchase + tokenResponse.tokenResult.Token;

                    _httpContextAccessor?.HttpContext?.Response?.Redirect(paymenPage);
                }
                else
                {
                    await _logger.ErrorAsync($"Error in get token from Barook payment: {JsonConvert.SerializeObject(tokenResponse)}");
                    _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error in get token from Barook payment", ex);
                _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);

            }
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new BarookSettings();
            _settingsService.SaveSetting(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Payments.Barook.SystemName"] = "Barook",
                ["ZarIran.Plugin.Payments.Barook.Name"] = "ZarIran Barook",
                ["ZarIran.Plugin.Payments.Barook.Configuration.SystemName"] = "Configuration",
                ["ZarIran.Plugin.Payments.Barook.Configuration.TerminalNumber"] = "Username",
                ["ZarIran.Plugin.Payments.Barook.Configuration.TerminalPassword"] = "Password",
                ["ZarIran.Plugin.Payments.Barook.Configuration.BaseUrl"] = "Base Url",
                ["ZarIran.Plugin.Payments.Barook.PaymentResult"] = "Payment Result:",
                ["ZarIran.Plugin.Payments.Barook.TrackingCode"] = "TrackingCode:",
                ["ZarIran.Plugin.Payments.Barook.ErrorInGetToken"] = "Error In Barook:",
                ["ZarIran.Plugin.Payments.Barook.PaymentWasSuccessful"] = "Payment was successful",
                ["ZarIran.Plugin.Payments.Barook.PaymentFailed"] = "خطا در پرداخت",
                ["ZarIran.Plugin.Payments.Barook.Error"] = "Error in payment.Try again",
                ["ZarIran.Payments.Barook.PaymentMethodDescription"] = "پرداخت اعتباری باروک",
                ["ZarIran.Plugin.Payments.Barook.OrderIsNotInPaidStatus"] = "Order is not paid status",

            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings

            await base.UninstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return;

            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var BarookSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.Barook.SystemName");
            var BarookName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.Barook.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.Barook.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payments.Barook.Configuration.SystemName");

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
                    ControllerName = "Barook",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }
                }
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = BarookName,
                SystemName = BarookSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = registerCustomerChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Barook/Configure";
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var processPaymentResult = new ProcessPaymentResult();

            return Task.FromResult(processPaymentResult);
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }


        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void not supported" } });
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported." } });
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring not supported" } });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            return Task.FromResult((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds >= 5.0);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            IList<string> errors = new List<string>();

            return Task.FromResult(errors);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form) => Task.FromResult(new ProcessPaymentRequest());

        public Type GetPublicViewComponent()
        {
            return null;
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund not supported" } });
        }


        #endregion

    }
}
