using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShoppingCartManagement
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
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.SystemName"] = "ShoppingCartManagement",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.Name"] = "Delsami ShoppingCartManagement",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.Username"] = "UserName",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.CreatedOnUtc"] = "CreatedOnUtc",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.UpdatedOnUtc"] = "UpdatedOnUtc",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.ReferralCode"] = "ReferralCode",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.TotalShoppingCartPrice"] = "TotalShoppingCartPrice",
                ["Delsami.Plugin.Misc.ShoppingCartManagement.CustomerRoleNames"] = "CustomerRoleNames",
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
