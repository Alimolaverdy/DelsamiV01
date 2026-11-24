using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.SMS.Infrastructure.Permissions;

namespace ZarIran.Plugin.Misc.SMS
{
    public class SMSPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public SMSPlugin(IWebHelper webHelper,
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

        #region Ctor

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/SMS/Configure";
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var smsSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.SMS.SystemName");
            var smsName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.SMS.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.SMS.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.SMS.Configuration.SystemName");
            var queuedSMSListName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.SMS.QueuedSMS.List");
            var queuedSMSListSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.SMS.QueuedSMS.List.SystemName");

            var zarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (zarIranSiteMapNode == null)
                zarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var carInfoChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "SMS",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                }
            };

            zarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = smsName,
                SystemName = smsSystemName,
                Visible = true,
                IconClass = "fas fa-sms",
                ChildNodes = carInfoChildNodes
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
                ["ZarIran.Plugin.Misc.SMS.SystemName"] = "SMS",
                ["ZarIran.Plugin.Misc.SMS.Name"] = "SMS",
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Misc.SMS.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.SMS.Configuration.SystemName"] = "SMSConfiguration",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMS.List.SystemName"] = "QueuedSMSList",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMS.List"] = "Queued SMSs",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMSs.Fields.SenderNumber"] = "Sender number",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMSs.Fields.ReceiverNumber"] = "Receiver number",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMSs.Fields.Body"] = "Message body",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMSs.Fields.CreatedDateTimeStr"] = "Created date",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMSs.Fields.SentDateTimeStr"] = "Sent date",
                ["ZarIran.Plugin.Misc.SMS.QueuedSMSs.Fields.SentTries"] = "Sent tries",
                ["ZarIran.Plugin.Misc.SMS.Configuration.ApiKey"] = "ApiKey",
                ["ZarIran.Plugin.Misc.SMS.Configuration.SenderNumber"] = "Number Of Sender",
                ["ZarIran.Plugin.Misc.SMS.Configuration.MaximumLengthOfSMS"] = "Maximum Length Of SMS",
                ["ZarIran.Plugin.Misc.SMS.Configuration.ExchangeName"] = "Exchange Name",
                ["ZarIran.Plugin.Misc.SMS.Configuration.RoutingKey"] = "Routing Key",
                ["ZarIran.Plugin.Misc.SMS.Configuration.Password"] = "Password Of SMS Service",

            });

            await _permissionService.InstallPermissionsAsync(new SMSPermission());

            var smsSettings = new SMSSettings
            {
                MaximumLengthOfSMS = 300
            };

            await _settingService.SaveSettingAsync(smsSettings);

            await base.InstallAsync();
        }

        #endregion
    }
}