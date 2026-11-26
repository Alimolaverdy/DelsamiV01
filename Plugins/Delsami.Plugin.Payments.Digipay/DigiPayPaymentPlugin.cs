using Microsoft.AspNetCore.Http;
using Delsami.Plugin.Payment.Core.Enums;
using Delsami.Plugin.Payment.Core.Infrastructure.Permissions;
using Delsami.Plugin.Payments.Digipay.Factories;
using Delsami.Plugin.Payments.Digipay.Infrastructure.Permissions;
using Delsami.Plugin.Payments.Digipay.Models.DigiPays;
using Delsami.Plugin.Payments.Digipay.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPaymentService = Delsami.Plugin.Payment.Core.Services.IPaymentService;

namespace Delsami.Plugin.Payments.Digipay;

public class DigiPayPaymentPlugin : BasePlugin, IPaymentMethod, IAdminMenuPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingsService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHelper _webHelper;
    private readonly IDigiPayService _digiPayService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IWorkContext _workContext;
    private readonly IBPGModelFactory _bpgModelFactory;
    private readonly IOrderService _orderService;
    private readonly INotificationService _notificationService;
    private readonly IPaymentService _paymentService;
    private readonly DigiPaySettings _digipaySettings;
    private readonly IPermissionService _permissionService;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public DigiPayPaymentPlugin(ILocalizationService localizationService,
                                      ISettingService settingsService,
                                      IHttpContextAccessor httpContextAccessor,
                                      IWebHelper webHelper,
                                      IDigiPayService digiPayService,
                                      IGenericAttributeService genericAttributeService,
                                      IWorkContext workContext,
                                      IBPGModelFactory bpgModelFactory,
                                      IOrderService orderService,
                                      INotificationService notificationService,
                                      IPaymentService paymentService,
                                      DigiPaySettings digipaySettings,
                                      IPermissionService permissionService,
                                       ILogger logger)
    {
        _localizationService = localizationService;
        _settingsService = settingsService;
        _httpContextAccessor = httpContextAccessor;
        _webHelper = webHelper;
        _digiPayService = digiPayService;
        _genericAttributeService = genericAttributeService;
        _workContext = workContext;
        _bpgModelFactory = bpgModelFactory;
        _orderService = orderService;
        _notificationService = notificationService;
        _paymentService = paymentService;
        _digipaySettings = digipaySettings;
        _permissionService = permissionService;
        _logger = logger;
    }

    #endregion

    #region Properties

    public bool SupportCapture => false;

    public bool SupportPartiallyRefund => false;

    public bool SupportRefund => true;

    public bool SupportVoid => false;

    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

    public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

    public bool SkipPaymentInfo => false;

    public async Task<string> GetPaymentMethodDescriptionAsync() => await _localizationService.GetResourceAsync("Delsami.Payments.Digipay.PaymentMethodDescription");

    #endregion

    #region Methods



    /// <summary>
    /// Install the plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        //settings
        var settings = new DigiPaySettings
        {
            DescriptionText = "<p>Mail Personal or Business Check, Cashier's Check or money order to:</p><p><br /><b>NOP SOLUTIONS</b> <br /><b>your address here,</b> <br /><b>New York, NY 10001 </b> <br /><b>USA</b></p><p>Notice that if you pay by Personal or Business Check, your order may be held for up to 10 days after we receive your check to allow enough time for the check to clear.  If you want us to ship faster upon receipt of your payment, then we recommend your send a money order or Cashier's check.</p><p>P.S. You can edit this text from admin panel.</p>"
        };

        _settingsService.SaveSetting(settings);

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Delsami.Plugin.BaseName"] = "Delsami",
            ["Delsami.Plugin.SystemName"] = "Delsami",
            ["Delsami.Plugin.Payments.Digipay.SystemName"] = "TorobPay",
            ["Delsami.Plugin.Payments.Digipay.Name"] = "Delsami Digipay",
            ["Delsami.Plugin.Payments.Digipay.Configuration.Username"] = "Username",
            ["Delsami.Plugin.Payments.Digipay.Configuration.Password"] = "Password",
            ["Delsami.Plugin.Payments.Digipay.Configuration.GrantType"] = "Grant Type",
            ["Delsami.Plugin.Payments.Digipay.Configuration.ClientId"] = "Client ID",
            ["Delsami.Plugin.Payments.Digipay.Configuration.ClientSecret"] = "Client Secret",
            ["Delsami.Plugin.Payments.Digipay.Configuration.BaseUrl"] = "Base Url",
            ["Delsami.Plugin.Payments.Digipay.Configuration.DigiPayVersion"] = "Digipay Version",
            ["Delsami.Plugin.Payments.Digipay.Configuration.DescriptionText"] = "Digipay BPG Description",
            ["Delsami.Plugin.Payments.Digipay.PaymentResult"] = "Payment Result:",
            ["Delsami.Plugin.Payments.Digipay.TrackingCode"] = "TrackingCode:",
            ["Delsami.Plugin.Payments.Digipay.AmountMismatch"] = "The paid amount does not match the order amount.Please check your payment:",
            ["Delsami.Plugin.Payments.Digipay.ErrorInGetToken"] = "Error In digipay:",
            ["Delsami.Plugin.Payments.Digipay.ErrorInCreateTicket"] = "Error in digipay",
            ["Delsami.Plugin.Payments.Digipay.PaymentWasSuccessful"] = "Payment was successful",
            ["Delsami.Plugin.Payments.Digipay.PaymentFailed"] = "Payment failed",
            ["Delsami.Plugin.Payments.Digipay.Message"] = "TrackingCode:",
            ["Delsami.Plugin.Payments.Digipay.Error"] = "Error in payment.Try again",
            ["Delsami.Plugin.Payments.Digipay.ErrorInRefund"] = "Error in digipay refund",
            ["Delsami.Plugin.Payments.Digipay.OrderIsNotInPaidStatus"] = "Order is not paid status",
            ["Delsami.Plugin.Payments.Digipay.Refund.SystemName"] = "RefundPaymnet",
            ["Delsami.Plugin.Payments.Digipay.Refund.Name"] = "Refund Paymnet",
            ["Delsami.Plugin.payments.Digipay.Refund.Title"] = "Refund payment",
        });

        await _permissionService.InstallPermissionsAsync(new DigiPayRefundPermission());

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
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins) && !await _permissionService.AuthorizeAsync(DigiPayRefundPermission.DigiPayRefund))
            return;

        var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
        var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
        var digipaySystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.SystemName");
        var digipayName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Name");
        var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Configuration");
        var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Configuration.SystemName");
        var refundSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Refund.SystemName");
        var refundName = await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.Refund.Name");

        var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

        if (DelsamiSiteMapNode == null)
            DelsamiSiteMapNode = new SiteMapNode
            {
                Title = baseName,
                SystemName = systemName,
                IconClass = "fa icon-plugins",
                Visible = true
            };

        var registerCustomerChildNodes = new List<SiteMapNode>();


        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
        {
            registerCustomerChildNodes.Add(new SiteMapNode
            {
                Title = configurationName,
                ControllerName = "Digipay",
                ActionName = "Configure",
                IconClass = "fas fa-cog",
                SystemName = configurationSystemName,
                Visible = true,
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "Area", "Admin" } }
            });
        }

        if (await _permissionService.AuthorizeAsync(DigiPayRefundPermission.DigiPayRefund) || await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
        {
            registerCustomerChildNodes.Add(new SiteMapNode
            {
                Title = refundName,
                ControllerName = "DigiPayRefund",
                ActionName = "RefundList",
                IconClass = "fas fa-cog",
                SystemName = refundSystemName,
                Visible = true,
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "Area", "Admin" } }
            });
        }


        DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
        {
            Title = digipayName,
            SystemName = digipaySystemName,
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
        return $"{_webHelper.GetStoreLocation()}Admin/DigiPay/Configure";
    }

    public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
    {
        var order = postProcessPaymentRequest.Order;

        if (order == null)
            throw new ArgumentNullException(nameof(order));

        var storeUrl = _webHelper.GetStoreLocation();
        if (!storeUrl.EndsWith("/"))
            storeUrl += "/";

        var errorPageUrl = $"{storeUrl}Checkout/PaymentError?orderId={order.Id}";

        try
        {
            var tokenResponse = await _digiPayService.GetTokenAsync();

            (bool CallIsSuccess, CreateTicketResponseModel CreateTicketResult) createTicketResponse = (false, null);

            var paymentProcessingIsOk = true;

            paymentProcessingIsOk = tokenResponse.CallIsSuccess && tokenResponse.TokenResult != null;

            var payment = new Payment.Core.Domain.Payment
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                SourceType = SourceType.Order,
                Gateway = Payment.Core.Enums.PaymentGateway.Digipay,
                Amount = order.OrderTotal,
                CreatedDateTime = DateTime.UtcNow,
                Status = PaymentStatus.Initial,
                TeminalNo = _digipaySettings.ClientId,
                PaymentType = PaymentType.CPG,
            };

            await _paymentService.InsertAsync(payment);

            if (paymentProcessingIsOk)
            {
                payment.AdditionalData = tokenResponse.TokenResult.access_token;

                var createTicketModel = await _bpgModelFactory.PrepareCreateTicketModelAsync(order, payment.Id);

                createTicketResponse = await _digiPayService.CreateTicketAsync(createTicketModel, tokenResponse.TokenResult.access_token);

                paymentProcessingIsOk = createTicketResponse.CallIsSuccess && createTicketResponse.CreateTicketResult != null;
            }
            else
                _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);

            if (paymentProcessingIsOk)
            {
                var orderNote = new OrderNote
                {
                    OrderId = order.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    DisplayToCustomer = false,
                    Note = $"DigiPay Ticket: {createTicketResponse.CreateTicketResult.ticket}",
                };

                await _orderService.InsertOrderNoteAsync(orderNote);

                payment.UrlId = createTicketResponse.CreateTicketResult.ticket;
                order.AuthorizationTransactionCode = createTicketResponse.CreateTicketResult.ticket;
                payment.Status = PaymentStatus.InProgress;
                await _paymentService.UpdateAsync(payment);

                _httpContextAccessor.HttpContext.Response.Redirect(createTicketResponse.CreateTicketResult.redirectUrl);

            }
            else
                _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error in get token from digipay payment", ex);

            _httpContextAccessor.HttpContext.Response.Redirect(errorPageUrl);
        }
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

    public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
    {
        var result = new RefundPaymentResult();
        var order = refundPaymentRequest.Order;

        if (order.PaymentStatus != Nop.Core.Domain.Payments.PaymentStatus.Paid)
        {
            result.AddError(await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.OrderIsNotInPaidStatus"));
            return result;
        }

        var payments = await _paymentService.GetPagedListAsync(order.Id);
        var token = await _digiPayService.GetTokenAsync();

        if (!token.CallIsSuccess)
        {
            result.AddError(await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.ErrorInRefund"));

            return result;
        }

        bool refundIsSuccess = true;

        foreach (var payment in payments)
        {
            if (payment.Gateway == Payment.Core.Enums.PaymentGateway.Digipay && payment.Completed)
            {
                var refundRequest = _bpgModelFactory.PrepareRefundRequest(payment, payment.TrackId);
                var type = "";

                if (payment.AdditionalData.Contains("Type:"))
                    type = payment.AdditionalData.Substring(6, 2);
                else
                    type = payment.AdditionalData;

                var refundResult = await _digiPayService.RefundAsync(refundRequest, token.TokenResult.access_token, int.Parse(type));

                if (!refundResult.CallIsSuccess)
                {
                    refundIsSuccess = false;
                    break;
                }


                payment.Status = PaymentStatus.Refunded;
                payment.Completed = true;
                payment.AdditionalData += $" - RefundTrackingCode: {refundResult.RefundResponse.Data.trackingCode}";
                await _paymentService.UpdateAsync(payment);
            }
        }


        if (!refundIsSuccess)
        {
            result.AddError(await _localizationService.GetResourceAsync("Delsami.Plugin.Payments.Digipay.ErrorInRefund"));

            return result;
        }

        result.NewPaymentStatus = Nop.Core.Domain.Payments.PaymentStatus.Refunded;

        return result;
    }

    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
    {
        throw new NotImplementedException();
    }

    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
    {
        return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported." } });
    }

    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
    {
        throw new NotImplementedException();
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

    #endregion
}
