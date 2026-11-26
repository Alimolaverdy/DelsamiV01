using Microsoft.AspNetCore.Http;
using Delsami.Plugin.Payment.Core.Enums;
using Delsami.Plugin.Payments.Saman.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
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
using System.Text.Json;
using System.Threading.Tasks;
using IPaymentService = Delsami.Plugin.Payment.Core.Services.IPaymentService;

namespace Delsami.Plugin.Payments.Saman
{
    public class SamanPaymentPlugin : BasePlugin, IPaymentMethod, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingsService;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly ISamanService _SamanService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly SamanSettings _SamanSettings;
        private readonly IPermissionService _permissionService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public SamanPaymentPlugin(ILocalizationService localizationService,
                                          ISettingService settingsService,
                                          IHttpContextAccessor httpContextAccessor,
                                          IWebHelper webHelper,
                                          ICustomerService customerService,
                                          ISamanService SamanService,
                                          IOrderService orderService,
                                          IPaymentService paymentService,
                                          SamanSettings SamanSettings,
                                          IPermissionService permissionService,
                                          ILogger logger)
        {
            _localizationService = localizationService;
            _settingsService = settingsService;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
            _SamanService = SamanService;
            _customerService = customerService;
            _orderService = orderService;
            _paymentService = paymentService;
            _SamanSettings = SamanSettings;
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

        public async Task<string> GetPaymentMethodDescriptionAsync() => await _localizationService.GetResourceAsync("Delsami.Payments.Saman.PaymentMethodDescription");

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
                    Gateway = PaymentGateway.Saman,
                    Amount = order.OrderTotal,
                    CreatedDateTime = DateTime.UtcNow,
                    Status = PaymentStatus.Initial,
                    PaymentType = PaymentType.IPG,
                    TeminalNo = "",

                };
                await _paymentService.InsertAsync(payment);

                var tokenResponse = await _SamanService.GetToken(order.Id, payment, customer.Username);

                await _logger.InformationAsync($"Saman Get Token response in start payment:{JsonSerializer.Serialize(tokenResponse)}");

                bool paymentProcessingIsOk = tokenResponse.CallIsSuccess && tokenResponse.tokenResult != null && tokenResponse.tokenResult.Token != null;

                if (paymentProcessingIsOk)
                {

                    var orderNote = new OrderNote
                    {
                        OrderId = order.Id,
                        CreatedOnUtc = DateTime.UtcNow,
                        DisplayToCustomer = false,
                        Note = $"Saman Ticket: {tokenResponse.tokenResult.Token}",
                    };

                    order.AuthorizationTransactionCode = tokenResponse.tokenResult.Token;
                    payment.Status = PaymentStatus.InProgress;

                    await _orderService.InsertOrderNoteAsync(orderNote);
                    await _orderService.UpdateOrderAsync(order);
                    await _paymentService.UpdateAsync(payment);

                    var paymenPage = _SamanSettings.BaseUrl + SamanPaymentEndpoints.Purchase + tokenResponse.tokenResult.Token;

                    _httpContextAccessor?.HttpContext?.Response?.Redirect(paymenPage);

                }
                else
                {

                    await _logger.ErrorAsync($"Error in get token from Saman payment, Error Code: {tokenResponse.tokenResult?.ErrorCode} - Error decription:{tokenResponse.tokenResult?.ErrorDescription}");
                    _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
                }

            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error in get token from Saman payment", ex);
                _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);

            }
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new SamanSettings();
            _settingsService.SaveSetting(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Payments.Saman.SystemName"] = "Saman",
                ["Delsami.Plugin.Payments.Saman.Name"] = "Delsami Saman",
                ["Delsami.Plugin.Payments.Saman.Configuration.TerminalNumber"] = "Username",
                ["Delsami.Plugin.Payments.Saman.Configuration.TerminalPassword"] = "Password",
                ["Delsami.Plugin.Payments.Saman.Configuration.BaseUrl"] = "Base Url",
                ["Delsami.Plugin.Payments.Saman.Configuration.SamanVersion"] = "Saman Version",
                ["Delsami.Plugin.Payments.Saman.Configuration.DescriptionText"] = "Saman Payment Description",
                ["Delsami.Plugin.Payments.Saman.PaymentResult"] = "Payment Result:",
                ["Delsami.Plugin.Payments.Saman.TrackingCode"] = "TrackingCode:",
                ["Delsami.Plugin.Payments.Saman.ErrorInGetToken"] = "Error In Saman:",
                ["Delsami.Plugin.Payments.Saman.PaymentWasSuccessful"] = "Payment was successful",
                ["Delsami.Plugin.Payments.Saman.PaymentFailed"] = "خطا در پرداخت",
                ["Delsami.Plugin.Payments.Saman.Error"] = "Error in payment.Try again",
                ["Delsami.Payments.Saman.PaymentMethodDescription"] = "پرداخت با درگاه بانک سامان",
                ["Delsami.Plugin.Payments.Saman.OrderIsNotInPaidStatus"] = "Order is not paid status",
                ["Delsami.Plugin.Payments.Saman.Configuration.TokenCacheMinutes"] = "Access Token cache in minutes (Default is 20)",
                ["Delsami.Plugin.Payments.Saman.Configuration.TimeOutSeconds"] = "TimeOut for operation in Seconds (Default is 30)",
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

            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var SamanSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Saman.SystemName");
            var SamanName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Saman.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Saman.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Saman.Configuration.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
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
                    ControllerName = "Saman",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }
                }
            };

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = SamanName,
                SystemName = SamanSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = registerCustomerChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Saman/Configure";
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
