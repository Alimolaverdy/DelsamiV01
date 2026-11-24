using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator
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
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var customPdfGeneratorSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.SystemName");
            var customPdfGeneratorName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.SystemName");

            var zarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (zarIranSiteMapNode == null)
                zarIranSiteMapNode = new SiteMapNode
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

            zarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = customPdfGeneratorName,
                SystemName = customPdfGeneratorSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = customPdfGeneratorChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(zarIranSiteMapNode);

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
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.Name"] = "CustomPdfGenerator",
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.SystemName"] = "CustomPdfGenerator",
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.SystemName"] = "CustomPdfGeneratorConfiguration",
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.ZarIranVendorId"] = "ZarIran vendor",
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.Configuration.BarcodeFormat"] = "Barcode Format",
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.Orders.Shipments.Print"] = "Print",
                ["ZarIran.Plugin.Misc.CustomPdfGenerator.Orders.Shipments.Download"] = "Download",
            });

            await base.InstallAsync();
        }
        #endregion
    }
}
