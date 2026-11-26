using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Models.WarehouseVendors;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Factories
{
    public interface IWarehouseVendorFactory
    {
        Task<WarehouseVendorSearchModel> PrepareWarehouseVendorSearchModelAsync(WarehouseVendorSearchModel searchModel);

        Task<WarehouseVendorListModel> PrepareWarehouseVendorListModelAsync(WarehouseVendorSearchModel searchModel, int? customerId = null);

        Task<WarehouseVendorModel> PrepareWarehouseVendorModelAsync(WarehouseVendorModel warehouseVendorModel, WarehouseVendor warehouseVendor);
    }
}
