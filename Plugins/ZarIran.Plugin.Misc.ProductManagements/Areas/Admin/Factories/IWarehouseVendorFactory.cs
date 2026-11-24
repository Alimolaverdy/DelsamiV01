using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.WarehouseVendors;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Factories
{
    public interface IWarehouseVendorFactory
    {
        Task<WarehouseVendorSearchModel> PrepareWarehouseVendorSearchModelAsync(WarehouseVendorSearchModel searchModel);

        Task<WarehouseVendorListModel> PrepareWarehouseVendorListModelAsync(WarehouseVendorSearchModel searchModel, int? customerId = null);

        Task<WarehouseVendorModel> PrepareWarehouseVendorModelAsync(WarehouseVendorModel warehouseVendorModel, WarehouseVendor warehouseVendor);
    }
}
