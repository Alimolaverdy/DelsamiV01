using DocumentFormat.OpenXml.Wordprocessing;
using Delsami.Plugin.Payments.ManagePaymentMethod.Infrastructure.Permissions;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Payments.ManagePaymentMethod
{
    public class PaymentMethodManagementPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public PaymentMethodManagementPlugin(IWebHelper webHelper,
                         ILocalizationService localizationService,
                         ISettingService settingService,
                         IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _settingService = settingService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var managePaymentMethodSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.SystemName");
            var managePaymentMethodName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.Configuration.SystemName");
            var managePaymentMethodListName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodRestriction.List");
            var managePaymentMethodListSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodRestriction.List.SystemName");
            var managePaymentMethodRoleListName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodCustomerRoleRestriction.List");
            var managePaymentMethodRoleListSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodCustomerRoleRestriction.List.SystemName");
            var paymentMethodDiscountListName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodDiscount.List");
            var paymentMethodDiscountListSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodDiscount.List.SystemName");
            var amountRestrictionSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodAmountRestriction.SystemName");
            var amountRestrictionTitle = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodAmountRestriction"); 
            var shippingRestrictionSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodShippingRestriction.SystemName");
            var shippingRestrictionTitle = await _localizationService.GetResourceAsync("Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodShippingRestriction");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var paymentMethodRestrictionChildNodes = new List<SiteMapNode>
            {
                 new SiteMapNode
                {
                    Title = managePaymentMethodListName,
                    ControllerName = "PaymentMethodRestriction",
                    ActionName = "Restrictions",
                    IconClass="fas fa-cog",
                    SystemName=managePaymentMethodListSystemName,
                    Visible = true
                },
                 new SiteMapNode
                {
                    Title = managePaymentMethodRoleListName,
                    ControllerName = "PaymentMethodCustomerRoleRestriction",
                    ActionName = "Restrictions",
                    IconClass="fas fa-cog",
                    SystemName=managePaymentMethodRoleListSystemName,
                    Visible = true
                },
                 new SiteMapNode
                {
                    Title = shippingRestrictionTitle,
                    ControllerName = "PaymentMethodRestriction",
                    ActionName = "GetShippingMethodRestrictions",
                    IconClass="fas fa-cog",
                    SystemName=shippingRestrictionSystemName,
                    Visible = true
                },
                new SiteMapNode
                {
                    Title = paymentMethodDiscountListName,
                    ControllerName = "PaymentMethodDiscount",
                    ActionName = "List",
                    IconClass="fas fa-cog",
                    SystemName=paymentMethodDiscountListSystemName,
                    Visible = true
                },
                new SiteMapNode
                {
                    Title = amountRestrictionTitle,
                    ControllerName = "PaymentMethodRestriction",
                    ActionName = "AmountRestrictions",
                    IconClass="fas fa-cog",
                    SystemName=amountRestrictionSystemName,
                    Visible = true
                },
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "PaymentMethodRestriction",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                }
            };

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = managePaymentMethodName,
                SystemName = managePaymentMethodSystemName,
                Visible = true,
                IconClass = "fas fa-sms",
                ChildNodes = paymentMethodRestrictionChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await _permissionService.InstallPermissionsAsync(new PaymentMethodStateProvinceRestrictionPermission());

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.Name"] = "Managage Payment Method",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.SystemName"] = "PaymentMethodSystemName",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.Configuration"] = "Configuration",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.Configuration.SystemName"] = "ManagagePaymentMethodConfigurationSystemName",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodRestriction.List.SystemName"] = "PaymentMethodRestrictionSystemName",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodRestriction.List"] = "Payment Method Restriction",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodDiscount.List.SystemName"] = "PaymentMethodDiscountSystemName",
                ["Delsami.Plugin.Paymanets.ManagePaymentMethod.PaymentMethodDiscount.List"] = "Payment Method Discount",
                ["Delsami.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.PaymentMethodSystemName"] = "Payment Method System Name",
                ["Delsami.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.DiscountPercent"] = "Discount percentage",
                ["Delsami.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.StartDateStr"] = "Start date",
                ["Delsami.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.EndDateStr"] = "End date",
                ["Delsami.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.AddNew"] = "Add new Discount",
                ["Delsami.Plugin.Payments.ManagePaymentMethod.PaymentMethodDiscount.BackToList"] = "Back to list",
            });

            await base.InstallAsync();
        }

        #endregion
    }
}