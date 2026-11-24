using DocumentFormat.OpenXml.Spreadsheet;
using ZarIran.Plugin.Misc.ExtendedOrder.Infrastructure.Permissions;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder
{
    internal class ExtendedOrderPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ExtendedOrderPlugin(ILocalizationService localizationService, IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Properties

        public bool HideInWidgetList => true;

        #endregion

        #region Methods

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(ExtendedOrderPermission.ManageExtendedOrder))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var extendedOrderSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedOrder.SystemName");
            var extendedOrderName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedOrder.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedOrder.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedOrder.Configuration.SystemName");
            var subStatusName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedOrder.SubStatusName");
            var subStatusSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedOrder.SubStatusName.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var extendedOrderChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "ExtendedOrder",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                },

                new SiteMapNode
                {
                    Title = subStatusName,
                    ControllerName = "SubStatus",
                    ActionName = "List",
                    IconClass="fas fa-cog",
                    SystemName=subStatusSystemName,
                    Visible = true
                },
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = extendedOrderName,
                SystemName = extendedOrderSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = extendedOrderChildNodes
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
                ["ZarIran.Plugin.Misc.ExtendedOrder.Name"] = "ExtendedOrder",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SystemName"] = "ExtendedOrder",
                ["ZarIran.Plugin.Misc.ExtendedOrder.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.ExtendedOrder.Configuration.SystemName"] = "ExtendedOrderConfiguration",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatusName"] = "subStatusName",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatusName.SystemName"] = "SubStatusSystemName",
                ["ZarIran.Plugin.Misc.ExtendedOrder.Configuration.OrderExpirationAsMinutes"] = "Order expiration (Minutes):",
                ["ZarIran.Plugin.Misc.ExtendedOrder.BriefAddress"] = "BriefAddress",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.List"] = "SubStatus list",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.FullName"] = "SubStatus Name",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.SystemName"] = "SubStatus SystemName",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.Add"] = "SubStatus Added",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.AddNew"] = "Add New SubStatus",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.BackToList"] = "Back To List",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.SubStatus"] = "SubStatus",
                ["ZarIran.Plugin.Misc.ExtendedOrder.SubStatus.Hint"] = "SubStatus Hint",
                ["Admin.Orders.Fields.OrderSubStatus.Change.ForAdvancedUsers"] = "Substatus Change Message",
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
        #endregion
    }
}
