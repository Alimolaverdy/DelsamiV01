using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements
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
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Name"] = "ExtendedVendor",
                ["ZarIran.Plugin.Misc.ExtendedVendor.SystemName"] = "ExtendedVendor",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Configuration.Initialize"] = "Initialize",
                ["ZarIran.Plugin.Misc.ExtendedVendor.GroupedProductOrderBy"] = "Order by",
                ["ZarIran.Plugin.Misc.ExtendedVendor.SortType"] = "Sort type",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Configuration.SystemName"] = "ExtendedVendorConfiguration",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Product"] = "Product",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Product.SystemName"] = "ExtendedVendorProduct",
                ["ZarIran.Plugin.Misc.ExtendedVendor.CustomProductDetailsModel.Distance"] = "Distance",
                ["ZarIran.Plugin.Misc.ExtendedVendor.CustomProductDetailsModel.Distance.LessThanOneKilometer"] = "Less than 1 KM",
                ["ZarIran.Plugin.Misc.ExtendedVendor.WarehouseVendorModel.WarehouseName"] = "Warehouse name",
                ["ZarIran.Plugin.Misc.ExtendedVendor.WarehouseVendorModel.VendorName"] = "Vendor name",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.Updated"] = "The warehouse-vendor has been updated successfully.",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.Added"] = "The new warehouse-vendor has been added successfully.",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.Deleted"] = "The   warehouse-vendor has been deleted successfully.",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.List"] = "Warehose vendors",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.List.SystemName"] = "WarehouseVendors",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.AddNew"] = "Add a new warehouse vendor",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.Edit"] = "Edit warehouse vendor",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.BackToList"] = "Back to warehouse vendor list",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.VendorIsDuplicated"] = "Vendor is duplicated",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseIsDuplicated"] = "Warehouse is duplicated",
                ["ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.ErrorMessage.WarehouseForVendorIsNotSpecified"] = "Warehouse for vendor is not specified",
                ["ZarIran.Plugin.Misc.ExtendedVendor.CustomProductDetailsModel.Distance.DistanceAsString"] = "{0} KM"
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
            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var extendedVendorSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.SystemName");
            var extendedVendorName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.Configuration.SystemName");
            var productName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.Product");
            var productSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.Product.SystemName");
            var warehouseVendorsName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.List");
            var warehouseVendorsSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedVendor.Warehousevendor.List.SystemName");
            var ProductManagementName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ProductManagements.Name");
            var ProductManagementSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ProductManagements.SystemName");
            var importReportsName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ProductManagements.ImportReports.Name");
            var importReportsSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ProductManagements.ImportReports.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
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

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = ProductManagementName,
                SystemName = ProductManagementSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = extendedVendorChildNodes
            });



            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("ZarIran.Plugin.Misc.ExtendedVendor");

            await base.UninstallAsync();

        }

        #endregion
    }
}
