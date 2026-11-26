using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CustomPdfGenerator
{
    public class CustomPdfGeneratorPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public CustomPdfGeneratorPlugin(ILocalizationService localizationService, IPermissionService permissionService)
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var customPdfGeneratorSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomPdfGenerator.SystemName");
            var customPdfGeneratorName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomPdfGenerator.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomPdfGenerator.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomPdfGenerator.Configuration.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var customPdfGeneratorChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "CustomPdfGenerator",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                }
            };

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = customPdfGeneratorName,
                SystemName = customPdfGeneratorSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = customPdfGeneratorChildNodes
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
                ["Delsami.Plugin.Misc.CustomPdfGenerator.Name"] = "CustomPdfGenerator",
                ["Delsami.Plugin.Misc.CustomPdfGenerator.SystemName"] = "CustomPdfGenerator",
                ["Delsami.Plugin.Misc.CustomPdfGenerator.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.CustomPdfGenerator.Configuration.SystemName"] = "CustomPdfGeneratorConfiguration",
                ["Delsami.Plugin.Misc.CustomPdfGenerator.Configuration.DelsamiVendorId"] = "Delsami vendor",
                ["Delsami.Plugin.Misc.CustomPdfGenerator.Configuration.BarcodeFormat"] = "Barcode Format",
                ["Delsami.Plugin.Misc.CustomPdfGenerator.Orders.Shipments.Print"] = "Print",
                ["Delsami.Plugin.Misc.CustomPdfGenerator.Orders.Shipments.Download"] = "Download",
            });

            await base.InstallAsync();
        }
        #endregion
    }
}
