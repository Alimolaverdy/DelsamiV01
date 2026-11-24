using ZarIran.Plugin.Payment.Core.Areas.Admin.Components;
using ZarIran.Plugin.Payment.Core.Infrastructure.Permissions;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payment.Core
{
    public class PaymentCorePlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public PaymentCorePlugin(IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISettingService settingService,
                                 IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _settingService = settingService;
            _permissionService = permissionService;
        }

        public bool HideInWidgetList => true;

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ZarIranPayment/Configure";
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == AdminWidgetZones.OrderDetailsBlock)
                return typeof(PaymentsForOrderViewComponent);

            return null;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            IList<string> widgets = new List<string> { AdminWidgetZones.OrderDetailsBlock };

            return Task.FromResult(widgets);
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
                ["ZarIran.Plugin.Payment.Core.Name"] = "ZarIran Payment",
                ["ZarIran.Plugin.Payment.Core.SystemName"] = "PaymentCore",
                ["ZarIran.Plugin.Payment.Core.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Payment.Core.Configuration.SystemName"] = "materialConfiguration",
                ["ZarIran.Plugin.Payment.Core.PaymentStatus.Completed"] = "Completed",
                ["ZarIran.Plugin.Payment.Core.Configuration.InProgress"] = "InProgress",
                ["ZarIran.Plugin.Payment.Core.Configuration.Failed"] = "Failed",
                ["ZarIran.Plugin.Payment.Core.PaymentStatus.Verified"] = "Verified",
                ["ZarIran.Plugin.Payment.Core.PaymentStatus.Initial"] = "Initial",
                ["ZarIran.Plugin.Payment.Core.PaymentStatus.InProgress"] = "InProgress",
                ["ZarIran.Plugin.Payment.Core.PaymentStatus.Failed"] = "Failed",
                ["ZarIran.Plugin.Payment.Core.Payments"] = "Payments",
                ["ZarIran.Plugin.Payment.Core.Payment.List"] = "Payment List",
                ["ZarIran.Plugin.Payment.Core.PaymentGateway.Pasargad"] = "Pasargad",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Id"] = "Payment Id",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.OrderId"] = "Order Id",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Gateway"] = "Gateway",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.TeminalNo"] = "Terminal No",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.ReferenceNumber"] = "Reference Number",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.TrackId"] = "Track Id",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.MaskedCardNumber"] = "Masked Card Number",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.HashedCardNumber"] = "Hashed Card Number",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.UrlId"] = "Url Id",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Amount"] = "Amount",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.AdditionalData"] = "Additional Data",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.CreatedDateTime"] = "Created On",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.VerifyDateTime"] = "Verified On",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.ConfirmDateTime"] = "Confirmed On",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Status"] = "Status",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.ResultCode"] = "Result Code",
                ["ZarIran.Plugin.Payment.Core.Admin.PaymentModel.Completed"] = "Completed",
            });

            await _permissionService.InstallPermissionsAsync(new PaymentCorePermission());

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(PaymentCorePermission.ManagePayment))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var paymentCoreSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Core.SystemName");
            var paymentCoreName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Core.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Core.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Core.Configuration.SystemName");
            var paymentListName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Core.Payment.List");
            var paymentListSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Payment.Core.Payment.List.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var paymentCoreChildNodes = new List<SiteMapNode>
            {
                //new SiteMapNode
                //{
                //    Title = configurationName,
                //    ControllerName = "PaymentCore",
                //    ActionName = "Configure",
                //    IconClass="fas fa-cog",
                //    SystemName=configurationSystemName,
                //    Visible = true
                //},
                 new SiteMapNode
                {
                    Title = paymentListName,
                    ControllerName = "Payment",
                    ActionName = "List",
                    IconClass="fas fa-cog",
                    SystemName=paymentListSystemName,
                    Visible = true
                },
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = paymentCoreName,
                SystemName = paymentCoreSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = paymentCoreChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("ZarIran.Plugin.Payment.Core");

            await base.UninstallAsync();

        }

        #endregion
    }
}
