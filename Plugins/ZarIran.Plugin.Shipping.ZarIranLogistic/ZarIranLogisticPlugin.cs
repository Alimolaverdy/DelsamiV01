using ZarIran.Plugin.Shipping.ZarIranLogistic.Infrastructures.Permissions;
using ZarIran.Plugin.Shipping.ZarIranLogistic.Services;
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

namespace ZarIran.Plugin.Shipping.ZarIranLogistic
{
    public class ZarIranLogisticPlugin : BasePlugin, IAdminMenuPlugin, IShippingRateComputationMethod
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IZarIranLogisticService _ZarIranLogistic;
        #endregion

        #region Ctor
        public ZarIranLogisticPlugin(ILocalizationService localizationService,
                                      IPermissionService permissionService,
                                      ISettingService settingService,
                                      IZarIranLogisticService ZarIranLogistic)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _ZarIranLogistic = ZarIranLogistic;
        }
        #endregion

        #region Methods

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(ZarIranLogisticPermission.ManageZarIranLogistic))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var ZarIranLogisticSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ZarIranLogistic.SystemName");
            var ZarIranLogisticName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ZarIranLogistic.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ZarIranLogistic.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.ZarIranLogistic.Configuration.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var ZarIranLogisticChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "ZarIranLogistic",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                }
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = ZarIranLogisticName,
                SystemName = ZarIranLogisticSystemName,
                Visible = true,
                IconClass = "fas fa-truck",
                ChildNodes = ZarIranLogisticChildNodes
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
                ["ZarIran.Plugin.Shipping.ZarIranLogistic.SystemName"] = "ZarIranLogistic",
                ["ZarIran.Plugin.Shipping.ZarIranLogistic.Name"] = "ZarIran Logistic",
                ["ZarIran.Plugin.Shipping.ZarIranLogistic.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Shipping.ZarIranLogistic.Configuration.SystemName"] = "ZarIranLogisticConfiguration",
                ["ZarIran.Plugin.Shipping.ZarIranLogistic.Price"] = "NeighbouringFiveToNineKgPostPrice",
                ["ZarIran.Plugins.Shipping.ZarIranLogistic.ShippingMethodId"] = " PostShippingMethod"

            });

            await _permissionService.InstallPermissionsAsync(new ZarIranLogisticPermission());

            if (_settingService.LoadSettingAsync<ZarIranLogisticSettings>() == null)
            {
                var setting = new ZarIranLogisticSettings { Price = 1_200_000, ShippingMethodId = 4 , PickupTruckPrice = 3_500_000_00 , VolumeMustCarryByPickupTruckInCM3 = 192_500 , WeightMustCarryByPickupTruckInGrams= 35_000 , OverSizeProductVolumeMustCarryByPickupTruckInCM3 = 70 };
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
                ShippingOptions = await _ZarIranLogistic.PrepareShippingOptionsAsync(getShippingOptionRequest)
            };
            return shippingOptionResponse;
        }

        #endregion
    }
}
