using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payment.Tejarat
{
    public class TejaratPlugin : BasePlugin, IPaymentMethod, IPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public TejaratPlugin(ILocalizationService localizationService,
                             IHttpContextAccessor httpContextAccessor,
                             IWebHelper webHelper,
                             IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
            _permissionService = permissionService;
        }

        #endregion

        #region Properties

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => true;

        #endregion

        #region Methods

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            CancelRecurringPaymentResult c = new CancelRecurringPaymentResult();

            c.AddError("Recurring payment not supported.");

            return Task.FromResult(c);
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            return Task.FromResult((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds >= 5.0);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            CapturePaymentResult capturePaymentResult = new CapturePaymentResult();
            capturePaymentResult.AddError("Capture method not supported.");

            return Task.FromResult(capturePaymentResult);
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Tejarat.Description");
        }

        public Type GetPublicViewComponent()
        {
            return null;
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            _httpContextAccessor?
                .HttpContext?
                .Response?
                .Redirect(_webHelper.GetStoreLocation() + "TejaratBank/Pay/" + postProcessPaymentRequest.Order.Id);

            return Task.CompletedTask;
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult p = new ProcessPaymentResult();
            p.AddError("Recurring payment not supported.");

            return Task.FromResult(p);
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult refund = new RefundPaymentResult();
            refund.AddError("Refund method not supported.");

            return Task.FromResult(refund);
        }

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

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult voidPay = new VoidPaymentResult();
            voidPay.AddError("Void method not supported.");

            return Task.FromResult(voidPay);
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var tejaratSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedOrder.SystemName");
            var tejaratName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Tejarat.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Tejarat.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Tejarat.Configuration.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var tejaratChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "Tejarat",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                }
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = tejaratName,
                SystemName = tejaratSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = tejaratChildNodes
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
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Payment.Tejarat.Name"] = "Tejarat Bank",
                ["ZarIran.Plugin.Payment.Tejarat.SystemName"] = "TejaratBank",
                ["ZarIran.Plugin.Payment.Tejarat.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Payment.Tejarat.Configuration.SystemName"] = "TejaratBankConfiguration",
                ["ZarIran.Plugin.Payment.Tejarat.Configuration.TerminalId"] = "Terminal",
                ["ZarIran.Plugin.Payment.Tejarat.Configuration.AcceptorId"] = "Acceptor Id",
                ["ZarIran.Plugin.Payment.Tejarat.Configuration.PassPhrase"] = "PassPhrase",
                ["ZarIran.Plugin.Payment.Tejarat.Configuration.RsaPublicKey"] = "RsaPublicKey",
                ["ZarIran.Plugin.Payment.Tejarat.Description"] = "Tejarat payment gateway description",

            });

            await base.InstallAsync();
        }

        public override string GetConfigurationPageUrl()
        {
            return this._webHelper.GetStoreLocation(null) + "Admin/Tejarat/Configure";
        }

        #endregion
    }
}
