using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CarInfo
{
    public class ShoppingCartPagePlugin : BasePlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public ShoppingCartPagePlugin(IWebHelper webHelper,
                             ISettingService settingService,
                             ILocalizationService localizationService,
                             IMessageTemplateService messageTemplateService,
                             EmailAccountSettings emailAccountSettings,
                             IScheduleTaskService scheduleTaskService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _emailAccountSettings = emailAccountSettings;
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Properties

        public bool HideInWidgetList => true;

        #endregion

        #region Methods

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.ContentAfter
            });
        }


        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ShoppingCartPage/Configure";
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
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.ShoppingCartPage.Name"] = "Shopping cart page",
                ["Delsami.Plugin.Misc.ShoppingCartPage.SystemName"] = "ShoppingCartPage",
                ["Delsami.Plugin.Misc.ShoppingCartPage.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.ShoppingCartPage.Configuration.SystemName"] = "ShoppingCartPageConfiguration",

            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            //await _settingService.DeleteSettingAsync<CarInfoSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.ShoppingCartPage");

            await base.UninstallAsync();
        }

        #endregion
    }
}
