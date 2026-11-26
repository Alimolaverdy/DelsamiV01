using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.TorobApi
{
    public class TorobApiPlugin : BasePlugin, IMiscPlugin,IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public TorobApiPlugin(IWebHelper webHelper,
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
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.TorobApi.Name"] = "Torob Api",
                ["Delsami.Plugin.Misc.TorobApi.SystemName"] = "TorobApiSystemName",
                ["Delsami.Plugin.Misc.TorobApi.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.TorobApi.Configuration.SystemName"] = "TorobApiConfigurationSystemName",
                ["Delsami.Plugin.Misc.TorobApi.Configuration.Token"] = "Torob Token",

            });

            //var smsSettings = new TorobApiSettings
            //{
            //    Token = "",
            //};

            //await _settingService.SaveSettingAsync(smsSettings);

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var torobApiSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.TorobApi.SystemName");
            var torobApiName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.TorobApi.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.TorobApi.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.TorobApi.Configuration.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var torobApiChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "TorobApi",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                }
            };

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = torobApiName,
                SystemName = torobApiSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = torobApiChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.TorobApi");

            await base.UninstallAsync();

        }
        #endregion
    }
}
