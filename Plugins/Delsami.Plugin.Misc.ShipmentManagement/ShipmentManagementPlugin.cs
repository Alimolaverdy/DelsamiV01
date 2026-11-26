using Delsami.Plugin.Misc.ShipmentManagement.Infrastructure.Permission;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement
{
    public class ShipmentManagementPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ShipmentManagementPlugin(IWebHelper webHelper,
                                        ISettingService settingService,
                                        ILocalizationService localizationService,
                                        IMessageTemplateService messageTemplateService,
                                        EmailAccountSettings emailAccountSettings,
                                        IScheduleTaskService scheduleTaskService,
                                        IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _emailAccountSettings = emailAccountSettings;
            _scheduleTaskService = scheduleTaskService;
            _permissionService = permissionService;
        }

        public bool HideInWidgetList => true;

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ShipmentManagement/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error in saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.ShipmentManagement.Name"] = "Shipment Management",
                ["Delsami.Plugin.Misc.ShipmentManagement.SystemName"] = "ShipmentManagement",
                ["Delsami.Plugin.Misc.ShipmentManagement.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.ShipmentManagement.Configuration.SystemName"] = "ShipmentManagementConfiguration",
                ["Delsami.Plugin.Misc.ShipmentManagement.ShipmentAdditionalData.DeliveryCodeSMSMessageTemplate"] = "Your delivery code is: {0}",
                ["Delsami.Plugin.Misc.ShipmentManagement.OrderStatus"] = "Deliverd To :{0}",
                ["Delsami.Plugin.Misc.ShipmentManagement.TrackingNumberAnnounceSoon"] = "Tracking Number Announce Soon",
                ["Delsami.Plugin.Misc.ShipmentManagement.CustomerOrders.ShipmentDate"] = "ShipmentDate",
                ["Delsami.Plugin.Misc.ShipmentManagement.Processing"] = "Processing",
                ["Delsami.Plugin.Misc.ShipmentManagement.Delivered"] = "Delivered",
            });


            await _permissionService.InstallPermissionsAsync(new DelsamiCustomPermissionForImportFile());


            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins) && !await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var shipmentManagementSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.SystemName");
            var shipmentManagementName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.Configuration.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var shipmentManagementChildNodes = new List<SiteMapNode>();


            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                shipmentManagementChildNodes.Add(new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "ShipmentManagement",
                    ActionName = "Configure",
                    IconClass = "fas fa-cog",
                    SystemName = configurationSystemName,
                    Visible = true
                });

            }

            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins) || await _permissionService.AuthorizeAsync(DelsamiCustomPermissionForImportFile.ImportShippmentTrackNumberFromFile))
            {

                shipmentManagementChildNodes.Add(new SiteMapNode
                {
                    SystemName = "Delsami.Plugin.Misc.ShipmentManagement.ShipmentTrackingNumberImport.ImportReport",
                    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.ReportTitle"),
                    ControllerName = "ShipmentTrackingNumberImport",
                    ActionName = "ImportList",
                    Visible = true,
                    IconClass = "fa fa-file-text",
                    RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "area", AreaNames.Admin } }
                });

                shipmentManagementChildNodes.Add(new SiteMapNode
                {
                    SystemName = "Delsami.Plugin.Misc.ShipmentManagement.ShipmentTrackingNumberImport.Import",
                    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.ImportTitle"),
                    ControllerName = "ShipmentTrackingNumberImport",
                    ActionName = "Import",
                    Visible = true,
                    IconClass = "fa fa-upload",
                    RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "area", AreaNames.Admin } }
                });
            }


            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = shipmentManagementName,
                SystemName = shipmentManagementSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = shipmentManagementChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            //await _settingService.DeleteSettingAsync<ShipmentManagementSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.ShipmentManagement");

            await base.UninstallAsync();
        }



        #endregion
    }
}
