using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.AdvRedirect.Infrastructure.Permissions;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;


namespace Nop.Plugin.Misc.AdvRedirect
{   //test advrediredt
    public class AdvRedirect : BasePlugin, IMiscPlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly WidgetSettings _widgetSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public bool HideInWidgetList => true;

        public AdvRedirect(IWebHelper webHelper,
                           ILocalizationService localizationService,
                           WidgetSettings widgetSettings,
                           ISettingService settingService,
                           IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _widgetSettings = widgetSettings;
            _settingService = settingService;
            _permissionService = permissionService;
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AdvRedirect/Configure";
        }

        public override async Task InstallAsync()
        {
            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(AdvRedirectDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(AdvRedirectDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            await _permissionService.InstallPermissionsAsync(new AdvRedirectPermission());

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Misc.AdvRedirect.Name"] = "Adv redirect",
                ["ZarIran.Plugin.Misc.AdvRedirect.SystemName"] = "AdvRedirect",
                ["ZarIran.Plugin.Misc.AdvRedirect.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.AdvRedirect.Configuration.SystemName"] = "AdvRedirectConfiguration",
                ["ZarIran.Plugin.Misc.AdvRedirect.RedirectionRule.List.SystemName"] = "RedirectionRuleList",
                ["ZarIran.Plugin.Misc.AdvRedirect.RedirectionRule.List"] = "Redirection Rule List",
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return AdvRedirectDefaults.TRACKING_VIEW_COMPONENT_NAME;
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(AdvRedirectPermission.ManageRedirectionRule))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var advRedirectSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.AdvRedirect.SystemName");
            var advRedirectName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.AdvRedirect.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.AdvRedirect.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.AdvRedirect.Configuration.SystemName");
            var redirectionRuleListName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.AdvRedirect.RedirectionRule.List");
            var redirectionRuleListSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.AdvRedirect.RedirectionRule.List.SystemName");

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
                    Title = redirectionRuleListName,
                    ControllerName = "RedirectionRule",
                    ActionName = "List",
                    IconClass="fas fa-cog",
                    SystemName=redirectionRuleListSystemName,
                    Visible = true
                },
                              new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "AdvRedirect",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                },
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = advRedirectName,
                SystemName = advRedirectSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = paymentCoreChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }
    }
}