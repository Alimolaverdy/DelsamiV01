using ZarIran.Plugin.Shipping.ManageShippingMethod.Infrastructures.Permissions;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod
{
    public class ManageShippingMethodPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ManageShippingMethodPlugin(ILocalizationService localizationService, IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.SystemName"] = "ManageShippingMethod",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.Name"] = "ZarIran ManageShippingMethod",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.List"] = "ManageShippingMethod List",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.List.SystemName"] = "ManageShippingMethodList",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.Configuration.SystemName"] = "ManageShippingMethodConfiguration",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.StateProvince"] = "State Province",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.StateProvince.Restrictions.Description"] = "Restrictions of State Province Shipping",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.ShippingMethodStateProvinceRestrictionModel.StateProvinceId"] = "State Province Id ",
                ["ZarIran.Plugin.Shipping.ManageShippingMethod.ShippingMethodStateProvinceRestrictionModel.ShippingMethodId"] = "Shipping Method Id "


            });

            await _permissionService.InstallPermissionsAsync(new ShippingMethodStateProvinceRestrictionPermission());

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(ShippingMethodStateProvinceRestrictionPermission.ManageShippingMethod))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var manageShippingMethodSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ManageShippingMethod.SystemName");
            var manageShippingMethodName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ManageShippingMethod.Name");
            var manageShippingMethodListName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ManageShippingMethod.List");
            var manageShippingMethodListSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ManageShippingMethod.List.SystemName");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ManageShippingMethod.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ManageShippingMethod.Configuration.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var registerManageShippingMethodChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "ManageShippingMethod",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }
                },

                 new SiteMapNode
                {
                    Title = manageShippingMethodListName,
                    ControllerName = "ManageShippingMethod",
                    ActionName = "Restrictions",
                    IconClass="fas fa-cog",
                    SystemName=manageShippingMethodListSystemName,
                    Visible = true
                },
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = manageShippingMethodName,
                SystemName = manageShippingMethodSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = registerManageShippingMethodChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        #endregion
    }
}
