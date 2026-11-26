using Delsami.Plugin.Misc.Core.Services;
using Delsami.Plugin.Shipping.Tipax.Services;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Tipax
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
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var tipaxSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.Tipax.SystemName");
            var tipaxName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.Tipax.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.Tipax.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Shipping.Tipax.Configuration.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
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

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = tipaxName,
                SystemName = tipaxSystemName,
                Visible = true,
                IconClass = "fas fa-truck",
                ChildNodes = tipaxChildNodes
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
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Shipping.Tipax.Name"] = "Tipax",
                ["Delsami.Plugin.Shipping.Tipax.SystemName"] = "Tipax",
                ["Delsami.Plugin.Shipping.Tipax.Configuration"] = "Configuration",
                ["Delsami.Plugin.Shipping.Tipax.Configuration.SystemName"] = "TipaxConfiguration",
                ["Delsami.Plugin.Shipping.Tipax.Configuration.TimeoutSeconds"] = "TimeoutSeconds",
                //["Delsami.Plugin.Shipping.Tipax.OrderDetails.TipaxRequest"] = "Tipax request",
                //["Delsami.Plugin.Shipping.Tipax.TipaxRequestModel.VendorName"] = "Vendor name",
                //["Delsami.Plugin.Shipping.Tipax.TipaxRequestModel.OriginAddress"] = "Origin address",
                //["Delsami.Plugin.Shipping.Tipax.TipaxRequestModel.DestinationAddress"] = "Destination address",
                //["Delsami.Plugin.Shipping.Tipax.TipaxRequestModel.Status"] = "Status",
                //["Delsami.Plugin.Shipping.Tipax.TipaxRequestModel.DetailsButton"] = "Details",
                //["Delsami.Plugin.Shipping.Tipax.Message.AloeykRequestHasAlreadyBeenCreated"] = "The tipax request has already been created",
                //["Delsami.Plugin.Shipping.Tipax.Message.AloeykRequestHasBeenCreatedSuccessfully"] = "The tipax request has been created successfully",
                //["Delsami.Plugin.Shipping.Tipax.TipaxRequestModel.CreateOrder"] = "Create order",
                //["Delsami.Plugin.Shipping.Tipax.TipaxRequestModel.AreYouSureToCreateTipaxRequest"] = "Are you sure to create a request ?"

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
