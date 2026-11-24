using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface IWarehouseVendorService
    {
        #region Signatures

        Task<WarehouseVendor> GetWarehouseVendorAsync(int id);

        Task<WarehouseVendor> GetWarehouseVendorByVendorIdAsync(int vendorId);

        Task InsertAsync(WarehouseVendor warehouseVendor);

        Task UpdateAsync(WarehouseVendor warehouseVendor);

        Task DeleteAsync(WarehouseVendor warehouseVendor);

        Task DeleteByWarehouseIdAsync(int warehouseId);

        Task DeleteByVendorIdAsync(int vendorId);

        Task<IPagedList<WarehouseVendor>> GetWarehouseVendorsAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IList<Vendor>> GetVendorByIdsAsync(IList<int> vendorIds);

        Task<IList<Warehouse>> GetWarehouseByIdsAsync(IList<int> warehouseIds);

        Task<bool> IsExistWarehouseAsync(int warehouseId, int? warehouseVendorId = null);

        Task<bool> IsExistVendorAsync(int vendorId, int? warehouseVendorId = null);

        #endregion
    }
}
