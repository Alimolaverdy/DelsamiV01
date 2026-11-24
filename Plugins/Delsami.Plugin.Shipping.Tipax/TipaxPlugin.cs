using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Shipping.Tipax.Services;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Tipax
{
    public class TipaxPlugin : BasePlugin, IAdminMenuPlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ITipaxService _tipaxService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public TipaxPlugin(ILocalizationService localizationService,
                             ITipaxService tipaxService,
                             IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _tipaxService = tipaxService;
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
            var tipaxSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.Tipax.SystemName");
            var tipaxName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.Tipax.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.Tipax.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Shipping.Tipax.Configuration.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var tipaxChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "Tipax",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }

                }
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = tipaxName,
                SystemName = tipaxSystemName,
                Visible = true,
                IconClass = "fas fa-truck",
                ChildNodes = tipaxChildNodes
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
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Shipping.Tipax.Name"] = "Tipax",
                ["ZarIran.Plugin.Shipping.Tipax.SystemName"] = "Tipax",
                ["ZarIran.Plugin.Shipping.Tipax.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Shipping.Tipax.Configuration.SystemName"] = "TipaxConfiguration",
                ["ZarIran.Plugin.Shipping.Tipax.Configuration.TimeoutSeconds"] = "TimeoutSeconds",
                //["ZarIran.Plugin.Shipping.Tipax.OrderDetails.TipaxRequest"] = "Tipax request",
                //["ZarIran.Plugin.Shipping.Tipax.TipaxRequestModel.VendorName"] = "Vendor name",
                //["ZarIran.Plugin.Shipping.Tipax.TipaxRequestModel.OriginAddress"] = "Origin address",
                //["ZarIran.Plugin.Shipping.Tipax.TipaxRequestModel.DestinationAddress"] = "Destination address",
                //["ZarIran.Plugin.Shipping.Tipax.TipaxRequestModel.Status"] = "Status",
                //["ZarIran.Plugin.Shipping.Tipax.TipaxRequestModel.DetailsButton"] = "Details",
                //["ZarIran.Plugin.Shipping.Tipax.Message.AloeykRequestHasAlreadyBeenCreated"] = "The tipax request has already been created",
                //["ZarIran.Plugin.Shipping.Tipax.Message.AloeykRequestHasBeenCreatedSuccessfully"] = "The tipax request has been created successfully",
                //["ZarIran.Plugin.Shipping.Tipax.TipaxRequestModel.CreateOrder"] = "Create order",
                //["ZarIran.Plugin.Shipping.Tipax.TipaxRequestModel.AreYouSureToCreateTipaxRequest"] = "Are you sure to create a request ?"

            });

            await base.InstallAsync();
        }

        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
			var products = getShippingOptionRequest.Items
			  .SelectMany(item => Enumerable.Repeat(item.Product, item.GetQuantity()))
			  .ToList();

			var shippingOptionResponse = new GetShippingOptionResponse()
            {
                ShippingOptions = await _tipaxService.PrepareShippingOptionsAsync(getShippingOptionRequest.Customer, getShippingOptionRequest.ShippingAddress, products)
            };

            return shippingOptionResponse;
        }

        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            return Task.FromResult<decimal?>(null);
        }

        public Task<IShipmentTracker> GetShipmentTrackerAsync()
        {
            return Task.FromResult<IShipmentTracker>(null);
        }

        #endregion

    }
}
