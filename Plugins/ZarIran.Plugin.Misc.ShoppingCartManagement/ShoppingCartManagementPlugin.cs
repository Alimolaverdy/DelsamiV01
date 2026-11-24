using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShoppingCartManagement
{
    public class ShoppingCartManagementPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {

        #region Fields

        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ShoppingCartManagementPlugin(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }
        #endregion

        #region Methods

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.SystemName"] = "ShoppingCartManagement",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.Name"] = "ZarIran ShoppingCartManagement",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.Username"] = "UserName",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.CreatedOnUtc"] = "CreatedOnUtc",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.UpdatedOnUtc"] = "UpdatedOnUtc",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.ReferralCode"] = "ReferralCode",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.TotalShoppingCartPrice"] = "TotalShoppingCartPrice",
                ["ZarIran.Plugin.Misc.ShoppingCartManagement.CustomerRoleNames"] = "CustomerRoleNames",
            });

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {;
            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
        #endregion

    }
}
