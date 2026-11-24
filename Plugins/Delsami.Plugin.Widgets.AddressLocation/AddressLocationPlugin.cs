using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarIran.Plugin.Widgets.AddressLocation.Components;
using ZarIran.Plugin.Widgets.AddressLocation.Infrastructure;
using ZarIran.Plugin.Widgets.AddressLocation.Services;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;

namespace ZarIran.Plugin.Widgets.AddressLocation
{
    public class AddressLocationPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IALAddressAttributeService _aLAddressAttributeService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public AddressLocationPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            IALAddressAttributeService aLAddressAttributeService,
            IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _aLAddressAttributeService = aLAddressAttributeService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                //PublicWidgetZones.AddressBottom,
                //AdminWidgetZones.CustomerAddressDetailsBottom,
                AdminWidgetZones.OrderBillingAddressDetailsBottom,
                AdminWidgetZones.OrderShippingAddressDetailsBottom,
                //AdminWidgetZones.OrderAddressDetailsBottom,
                //"admin_address_attributes_bottom"
            });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/AddressLocation/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            //if (/*widgetZone == PublicWidgetZones.AddressBottom ||*/
            //    widgetZone == AdminWidgetZones.CustomerAddressDetailsBottom ||
            //    widgetZone == AdminWidgetZones.OrderAddressDetailsBottom)
            //    return typeof(EditableLocationAddressViewComponent);
            
            if (widgetZone == AdminWidgetZones.OrderShippingAddressDetailsBottom)
                return typeof(ShowShippingLocationAddressViewComponent);

            else if (widgetZone == AdminWidgetZones.OrderBillingAddressDetailsBottom)
                return typeof(ShowBillingLocationAddressViewComponent);
            else
                return null;

        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "Zar Iran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Widgets.AddressLocation.Button.FindMyLocation"] = "Find My Location",
                ["ZarIran.Plugin.Widgets.AddressLocation.Configuration.LatitudeAddressAttributeId"] = "Address attribute for latitude",
                ["ZarIran.Plugin.Widgets.AddressLocation.Configuration.LongitudeAddressAttributeId"] = "Address attribute for longitude",
                ["ZarIran.Plugin.Widgets.AddressLocation.Configuration.CityAddressAttributeId"] = "Address attribute for city",
                ["ZarIran.Plugin.Widgets.AddressLocation.Configuration.MainAddressAttributeId"] = "Address attribute for main address",
                ["ZarIran.Plugin.Widgets.AddressLocation.Configuration.NeshanApiKey"] = "Neshan api key",
                ["ZarIran.Plugin.Widgets.AddressLocation.Name"] = "AddressLocation",
                ["ZarIran.Plugin.Widgets.AddressLocation.SystemName"] = "AddressLocation",
                ["ZarIran.Plugin.Widgets.AddressLocation.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Widgets.AddressLocation.Configuration.SystemName"] = "AddressLocationConfiguration",
                ["ZarIran.Plugin.Widgets.AddressLocation.LocateByState"] = "LocateByState",
                ["ZarIran.Plugin.Widgets.AddressLocation.InvalidStateProvince"] = "InvalidStateProvince",
                ["ZarIran.Plugin.Widgets.AddressLocation.SearchTermTitle"] = "SearchTermTitle",
                ["ZarIran.Plugin.Widgets.AddressLocation.SearchTermPlaceHolder"] = "SearchTermPlaceHolder"
            });

            var latAddressAttribute = await _aLAddressAttributeService.GetAddressAttributeByNameAsync(AddressLocationConstants.LATITUDE_NAME);

            if (latAddressAttribute == null)
                await _aLAddressAttributeService.InsertLatAddressAttribute();
            else
            {
                latAddressAttribute.DisplayOrder = int.MaxValue - 1;
                await _aLAddressAttributeService.UpdateAddressAttributeAsync(latAddressAttribute);
            }

            var lngAddressAttribute = await _aLAddressAttributeService.GetAddressAttributeByNameAsync(AddressLocationConstants.LONGITUDE_NAME);

            if (lngAddressAttribute == null)
                await _aLAddressAttributeService.InsertLngAddressAttribute();
            else
            {
                lngAddressAttribute.DisplayOrder = int.MaxValue;
                await _aLAddressAttributeService.UpdateAddressAttributeAsync(lngAddressAttribute);
            }

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<AddressLocationSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("ZarIran.Plugin.Widgets.AddressLocation");

            await base.UninstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var carInfoSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Widgets.AddressLocation.SystemName");
            var carInfoName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Widgets.AddressLocation.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Widgets.AddressLocation.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Widgets.AddressLocation.Configuration.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var carInfoChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "AddressLocation",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                }
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = carInfoName,
                SystemName = carInfoSystemName,
                Visible = true,
                IconClass = "fas fa-map-marker-alt",
                ChildNodes = carInfoChildNodes
            });

            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        #endregion
    }
}
