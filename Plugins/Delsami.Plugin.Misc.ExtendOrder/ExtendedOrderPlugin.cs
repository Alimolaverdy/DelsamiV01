using DocumentFormat.OpenXml.Spreadsheet;
using Delsami.Plugin.Misc.ExtendedOrder.Infrastructure.Permissions;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder
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
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var extendedOrderSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedOrder.SystemName");
            var extendedOrderName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedOrder.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedOrder.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedOrder.Configuration.SystemName");
            var subStatusName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedOrder.SubStatusName");
            var subStatusSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedOrder.SubStatusName.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
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

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = extendedOrderName,
                SystemName = extendedOrderSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = extendedOrderChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.ExtendedOrder.Name"] = "ExtendedOrder",
                ["Delsami.Plugin.Misc.ExtendedOrder.SystemName"] = "ExtendedOrder",
                ["Delsami.Plugin.Misc.ExtendedOrder.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.ExtendedOrder.Configuration.SystemName"] = "ExtendedOrderConfiguration",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatusName"] = "subStatusName",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatusName.SystemName"] = "SubStatusSystemName",
                ["Delsami.Plugin.Misc.ExtendedOrder.Configuration.OrderExpirationAsMinutes"] = "Order expiration (Minutes):",
                ["Delsami.Plugin.Misc.ExtendedOrder.BriefAddress"] = "BriefAddress",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.List"] = "SubStatus list",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.FullName"] = "SubStatus Name",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.SystemName"] = "SubStatus SystemName",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.Add"] = "SubStatus Added",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.AddNew"] = "Add New SubStatus",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.BackToList"] = "Back To List",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.SubStatus"] = "SubStatus",
                ["Delsami.Plugin.Misc.ExtendedOrder.SubStatus.Hint"] = "SubStatus Hint",
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
