using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement
{
    public class ProductManagementPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ProductManagementPlugin(IWebHelper webHelper,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ExtendedVendor/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.ExtendedVendor.Name"] = "ExtendedVendor",
                ["Delsami.Plugin.Misc.ExtendedVendor.SystemName"] = "ExtendedVendor",
                ["Delsami.Plugin.Misc.ExtendedVendor.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.ExtendedVendor.Configuration.Initialize"] = "Initialize",
                ["Delsami.Plugin.Misc.ExtendedVendor.GroupedProductOrderBy"] = "Order by",
                ["Delsami.Plugin.Misc.ExtendedVendor.SortType"] = "Sort type",
                ["Delsami.Plugin.Misc.ExtendedVendor.Configuration.SystemName"] = "ExtendedVendorConfiguration",
                ["Delsami.Plugin.Misc.ExtendedVendor.Product"] = "Product",
                ["Delsami.Plugin.Misc.ExtendedVendor.Product.SystemName"] = "ExtendedVendorProduct",
                ["Delsami.Plugin.Misc.ExtendedVendor.CustomProductDetailsModel.Distance"] = "Distance",
                ["Delsami.Plugin.Misc.ExtendedVendor.CustomProductDetailsModel.Distance.LessThanOneKilometer"] = "Less than 1 KM",
                ["Delsami.Plugin.Misc.ExtendedVendor.WarehouseVendorModel.WarehouseName"] = "Warehouse name",
                ["Delsami.Plugin.Misc.ExtendedVendor.WarehouseVendorModel.VendorName"] = "Vendor name",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.Updated"] = "The warehouse-vendor has been updated successfully.",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.Added"] = "The new warehouse-vendor has been added successfully.",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.Deleted"] = "The   warehouse-vendor has been deleted successfully.",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.List"] = "Warehose vendors",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.List.SystemName"] = "WarehouseVendors",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.AddNew"] = "Add a new warehouse vendor",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.Edit"] = "Edit warehouse vendor",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.BackToList"] = "Back to warehouse vendor list",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.VendorIsDuplicated"] = "Vendor is duplicated",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseIsDuplicated"] = "Warehouse is duplicated",
                ["Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseForVendorIsNotSpecified"] = "Warehouse for vendor is not specified",
                ["Delsami.Plugin.Misc.ExtendedVendor.CustomProductDetailsModel.Distance.DistanceAsString"] = "{0} KM"
            });

            await base.InstallAsync();

            //await _vendorCustomerDistanceService.InitializeWarehouseAddressCustomerDistancesAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }
            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var extendedVendorSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.SystemName");
            var extendedVendorName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Configuration.SystemName");
            var productName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Product");
            var productSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Product.SystemName");
            var warehouseVendorsName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.List");
            var warehouseVendorsSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ExtendedVendor.Warehousevendor.List.SystemName");
            var ProductManagementName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ProductManagement.Name");
            var ProductManagementSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ProductManagement.SystemName");
            var importReportsName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ProductManagement.ImportReports.Name");
            var importReportsSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ProductManagement.ImportReports.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var extendedVendorChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = configurationName,
                    ControllerName = "ProductManagement",
                    ActionName = "Configure",
                    IconClass="fas fa-cog",
                    SystemName=configurationSystemName,
                    Visible = true
                },

                new SiteMapNode
                {
                    Title = warehouseVendorsName,
                    ControllerName = "WarehouseVendor",
                    ActionName = "List",
                    IconClass="far fa-dot-circle",
                    SystemName=warehouseVendorsSystemName,
                    Visible = true
                },
            }; 

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = ProductManagementName,
                SystemName = ProductManagementSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = extendedVendorChildNodes
            });



            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.ExtendedVendor");

            await base.UninstallAsync();

        }

        #endregion
    }
}
