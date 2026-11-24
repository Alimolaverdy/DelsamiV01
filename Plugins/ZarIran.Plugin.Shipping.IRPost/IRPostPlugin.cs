using ZarIran.Plugin.Shipping.IRPost.Infrastructures.Permissions;
using ZarIran.Plugin.Shipping.IRPost.Services;
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

namespace ZarIran.Plugin.Shipping.IRPost
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
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var IRPostSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.SystemName");
            var IRPostName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.Configuration.SystemName");
            var priceRangeName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRangeModel.List");
            var priceRangeSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.IRPost.PriceRangeModel.List.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
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

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = IRPostName,
                SystemName = IRPostSystemName,
                Visible = true,
                IconClass = "fas fa-truck",
                ChildNodes = IRPostChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Shipping.IRPost.SystemName"] = "IRPost",
                ["ZarIran.Plugin.Shipping.IRPost.Name"] = "ZarIran IRPost",
                ["ZarIran.Plugin.Shipping.IRPost.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Shipping.IRPost.Configuration.SystemName"] = "IRPostConfiguration",
                ["ZarIran.Plugin.Shipping.IRPost.NeighbouringProvinces"] = "NeighbouringProvinces",
                ["ZarIran.Plugin.Shipping.IRPost.NeighbouringZeroToOneKgPostPrice"] = "NeighbouringZeroToOneKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NonNeighbouringZeroToOneKgPostPrice"] = "NonNeighbouringZeroToOneKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NeighbouringOneToTwoKgPostPrice"] = "NeighbouringOneToTwoKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NonNeighbouringOneToTwoKgPostPrice"] = "NonNeighbouringOneToTwoKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NeighbouringTwoToThreeKgPostPrice"] = "NeighbouringTwoToThreeKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NonNeighbouringTwoToThreeKgPostPrice"] = "NonNeighbouringTwoToThreeKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NeighbouringThreeToFiveKgPostPrice"] = "NeighbouringThreeToFiveKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NonNeighbouringThreeToFiveKgPostPrice"] = "NonNeighbouringThreeToFiveKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NeighbouringFiveToNineKgPostPrice"] = "NeighbouringFiveToNineKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NonNeighbouringFiveToNineKgPostPrice"] = "NonNeighbouringFiveToNineKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.NeighbouringMoreThanNineKgPostPrice"] = "NeighbouringMoreThanNineKgPostPrice",
                ["ZarIran.Plugin.Shipping.IRPost.PostSettings"] = "PostSettings",
                ["ZarIran.Plugin.Shipping.IRPost.State.Error"] = " Destination state/province is not specified",
                ["ZarIran.Plugins.Shipping.IRPost.PostShippingMethod"] = " PostShippingMethod"

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
