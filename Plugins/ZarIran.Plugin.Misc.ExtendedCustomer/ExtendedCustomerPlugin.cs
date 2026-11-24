using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedCustomer
{
    public class ExtendedCustomerPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ExtendedCustomerPlugin(ILocalizationService localizationService,
                                      EmailAccountSettings emailAccountSettings,
                                      IMessageTemplateService messageTemplateService,
                                      IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _emailAccountSettings = emailAccountSettings;
            _messageTemplateService = messageTemplateService;
            _permissionService = permissionService;
        }

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


            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "Nob Parts",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.SystemName"] = "ExtendedCustomer",
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Configuration.SystemName"] = "ExtendedCustomerConfiguration",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Account.CustomerShebaNumbers"] = "Sheba numbers",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Account.CustomerShebaNumbers.AddNew"] = "Add new sheba number",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Account.CustomerShebaNumbers.Edit"] = "Edit sheba number",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Account.CustomerShebaNumbers"] = "Sheba numbers",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Account.CustomerShebaNumbers.AddNew"] = "Add new sheba number",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.Account.CustomerShebaNumbers.Edit"] = "Edit sheba number",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.CustomerShebaNumber.Number"] = "Number",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.CustomerShebaNumber.Added"] = "Sheba number added successfully",
                ["ZarIran.Plugin.Misc.ExtendedCustomer.CustomerShebaNumber.Updated"] = "Sheba number updated successfully",

            });

            await base.InstallAsync();
        }

        #endregion

        #region Utilities

        #endregion
    }
}
