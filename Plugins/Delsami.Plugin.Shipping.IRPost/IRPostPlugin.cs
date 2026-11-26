using Delsami.Plugin.Shipping.IRPost.Infrastructures.Permissions;
using Delsami.Plugin.Shipping.IRPost.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost
{
    public class IRPostPlugin : BasePlugin, IAdminMenuPlugin, IShippingRateComputationMethod
    {

        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IIRPostService _irPostService;

        #endregion

        #region Ctor

        public IRPostPlugin(ILocalizationService localizationService,
                             IPermissionService permissionService,
                             ISettingService settingService,
                             IIRPostService iRPostService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _irPostService = iRPostService;
        }

        #endregion

        #region Methods

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(IRPostPermission.ManageIRPost))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var IRPostSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.IRPost.SystemName");
            var IRPostName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.IRPost.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.IRPost.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.IRPost.Configuration.SystemName");
            var priceRangeName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.IRPost.PriceRangeModel.List");
            var priceRangeSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.IRPost.PriceRangeModel.List.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var IRPostChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = priceRangeName,
                    ControllerName = "PriceRange",
                    ActionName = "List",
                    IconClass="fas fa-circle",
                    SystemName=priceRangeSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                },
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "IRPost",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                }
            };

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = IRPostName,
                SystemName = IRPostSystemName,
                Visible = true,
                IconClass = "fas fa-truck",
                ChildNodes = IRPostChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Shipping.IRPost.SystemName"] = "IRPost",
                ["Delsami.Plugin.Shipping.IRPost.Name"] = "Delsami IRPost",
                ["Delsami.Plugin.Shipping.IRPost.Configuration"] = "Configuration",
                ["Delsami.Plugin.Shipping.IRPost.Configuration.SystemName"] = "IRPostConfiguration",
                ["Delsami.Plugin.Shipping.IRPost.NeighbouringProvinces"] = "NeighbouringProvinces",
                ["Delsami.Plugin.Shipping.IRPost.NeighbouringZeroToOneKgPostPrice"] = "NeighbouringZeroToOneKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NonNeighbouringZeroToOneKgPostPrice"] = "NonNeighbouringZeroToOneKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NeighbouringOneToTwoKgPostPrice"] = "NeighbouringOneToTwoKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NonNeighbouringOneToTwoKgPostPrice"] = "NonNeighbouringOneToTwoKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NeighbouringTwoToThreeKgPostPrice"] = "NeighbouringTwoToThreeKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NonNeighbouringTwoToThreeKgPostPrice"] = "NonNeighbouringTwoToThreeKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NeighbouringThreeToFiveKgPostPrice"] = "NeighbouringThreeToFiveKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NonNeighbouringThreeToFiveKgPostPrice"] = "NonNeighbouringThreeToFiveKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NeighbouringFiveToNineKgPostPrice"] = "NeighbouringFiveToNineKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NonNeighbouringFiveToNineKgPostPrice"] = "NonNeighbouringFiveToNineKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.NeighbouringMoreThanNineKgPostPrice"] = "NeighbouringMoreThanNineKgPostPrice",
                ["Delsami.Plugin.Shipping.IRPost.PostSettings"] = "PostSettings",
                ["Delsami.Plugin.Shipping.IRPost.State.Error"] = " Destination state/province is not specified",
                ["Delsami.Plugins.Shipping.IRPost.PostShippingMethod"] = " PostShippingMethod"

            });

            await _permissionService.InstallPermissionsAsync(new IRPostPermission());

            if (_settingService.LoadSettingAsync<IRPostSettings>() == null)
            {
                var setting = new IRPostSettings { };
                await _settingService.SaveSettingAsync(setting);
            }
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public async Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            return await Task.FromResult<decimal?>(null);
        }

        public async Task<IShipmentTracker> GetShipmentTrackerAsync()
        {
            return await Task.FromResult<IShipmentTracker>(null);
        }

        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            var shippingOptionResponse = new GetShippingOptionResponse()
            {
                ShippingOptions = await _irPostService.PrepareShippingOptionsAsync(getShippingOptionRequest)
            };

            return shippingOptionResponse;
        }
        #endregion
    }
}
