using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.PriceEngine
{
    public class PriceEnginePlugin : BasePlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        public PriceEnginePlugin(ILocalizationService localizationService, IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
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
                ["ZarIran.Plugin.Misc.PriceEngine.Name"] = "ZarIran PriceEngine",
                ["ZarIran.Plugin.Misc.PriceEngine.SystemName"] = "ZarIranPriceEngine",
                ["ZarIran.Plugin.Misc.PriceEngine.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.PriceEngine.Configuration.SystemName"] = "CoreConfiguration",

            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync("ZarIran.Plugin.Misc.PriceEngine");

            await base.UninstallAsync();
        }
    }
}
