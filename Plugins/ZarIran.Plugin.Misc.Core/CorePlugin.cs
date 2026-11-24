using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Components;

namespace ZarIran.Plugin.Misc.Core
{
    public class CorePlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public CorePlugin(ILocalizationService localizationService, IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Properties

        public bool HideInWidgetList => true;

        #endregion

        #region Methods

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.ContentAfter)
                return typeof(ToEnglishNumberViewComponent);
            else
                return null;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
           {
               PublicWidgetZones.ContentAfter
           });
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
                ["ZarIran.Plugin.Misc.Core.Name"] = "ZarIran Core",
                ["ZarIran.Plugin.Misc.Core.SystemName"] = "ZarIranCore",
                ["ZarIran.Plugin.Misc.Core.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.Core.Configuration.SystemName"] = "CoreConfiguration",
                ["ZarIran.Plugin.Misc.Core.Configuration.ManualGoldPrice"] = "ManualGoldPrice",
                ["ZarIran.Plugin.Misc.Core.Configuration.UseManualGoldPrice"] = "UseManualGoldPrice",
                ["ZarIran.Plugin.Misc.Core.Configuration.GoldPriceAdjustment"] = "GoldPriceAdjustment",
                ["ZarIran.Plugin.Misc.Core.Configuration.NationalCodeAttributeId"] = "NationalCodeAttributeId",
                ["ZarIran.Plugin.Misc.Core.Configuration.AllProductsCategoryId"] = "AllProductsCategoryId",
                ["ZarIran.Plugin.Misc.Core.Configuration.Tax"] = "Tax",
                ["ZarIran.Plugin.Misc.Core.Configuration.AllowGoldPriceAdjustment"] = "AllowGoldPriceAdjustment",
                ["ZarIran.plugin.misc.sms.name "] = "ZarIran SMS"

            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync("ZarIran.Plugin.Misc.Core");

            await base.UninstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }

            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var coreSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.Core.SystemName");
            var coreName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.Core.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.Core.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.Core.Configuration.SystemName");

            var zarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (zarIranSiteMapNode == null)
                zarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var coreChildNodes = new List<SiteMapNode>

             {
                 new SiteMapNode
                 {
                     Title = configurationName,
                     ControllerName = "Core",
                     ActionName = "Configure",
                     IconClass="fas fa-cog",
                     SystemName=configurationSystemName,
                     Visible = true
                 }
             };

            zarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = coreName,
                SystemName = coreSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = coreChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(zarIranSiteMapNode);
        }

        #endregion
    }
}
