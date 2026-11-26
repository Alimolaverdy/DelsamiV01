using Nop.Services.Localization;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.CategoryEnhancer
{
    public class CategoryEnhancerPlugin : BasePlugin
    {
        private readonly ILocalizationService _localizationService;

        public CategoryEnhancerPlugin(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public async override Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {

                ["Delsami.Plugin.Misc.CategoryEnhancer.Icon"] = "Icon"
            });

            await base.InstallAsync();
        }

        public async override Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }
}