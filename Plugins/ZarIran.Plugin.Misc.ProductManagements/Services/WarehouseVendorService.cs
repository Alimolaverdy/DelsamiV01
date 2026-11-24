using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Shipping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class WarehouseVendorService : IWarehouseVendorService
    {
        #region Fields

        private readonly IRepository<WarehouseVendor> _warehouseVendorRepo;
        private readonly IRepository<Warehouse> _warehouseRepo;
        private readonly IRepository<Vendor> _vendorRepo;

        #endregion

        #region Ctor

        public WarehouseVendorService(IRepository<WarehouseVendor> warehouseVendorRepo,
                                      IRepository<Warehouse> warehouseRepo,
                                      IRepository<Vendor> vendorRepo)
        {
            _warehouseVendorRepo = warehouseVendorRepo;
            _warehouseRepo = warehouseRepo;
            _vendorRepo = vendorRepo;
        }

        #endregion

        #region Methods

        public async Task<WarehouseVendor> GetWarehouseVendorAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("id");

            return await _warehouseVendorRepo.GetByIdAsync(id);
        }

        public async Task<WarehouseVendor> GetWarehouseVendorByVendorIdAsync(int vendorId)
        {
            return await _warehouseVendorRepo.Table.FirstOrDefaultAsync(f => f.VendorId == vendorId);
        }

        public async Task InsertAsync(WarehouseVendor warehouseVendor)
        {
            if (warehouseVendor == null)
                throw new ArgumentNullException(nameof(warehouseVendor));

            await _warehouseVendorRepo.InsertAsync(warehouseVendor);
        }

        public async Task UpdateAsync(WarehouseVendor warehouseVendor)
        {
            if (warehouseVendor == null)
                throw new ArgumentNullException(nameof(warehouseVendor));

            await _warehouseVendorRepo.UpdateAsync(warehouseVendor);
        }


        public async Task DeleteAsync(WarehouseVendor warehouseVendor)
        {
            if (warehouseVendor == null)
                throw new ArgumentNullException(nameof(warehouseVendor));

            await _warehouseVendorRepo.DeleteAsync(warehouseVendor);
        }

        public async Task DeleteByWarehouseIdAsync(int warehouseId)
        {
            await _warehouseVendorRepo.DeleteAsync(x => x.WarehouseId == warehouseId);
        }

        public async Task DeleteByVendorIdAsync(int vendorId)
        {
            await _warehouseVendorRepo.DeleteAsync(x => x.VendorId == vendorId);
        }

        public virtual async Task<IPagedList<WarehouseVendor>> GetWarehouseVendorsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var warehouseVendors = await _warehouseVendorRepo.GetAllPagedAsync(query =>
            {
                query = query.OrderByDescending(v => v.CreatedDateTime);

                return query;
            }, pageIndex, pageSize);

            return warehouseVendors;
        }

        public async Task<IList<Vendor>> GetVendorByIdsAsync(IList<int> vendorIds)
        {
            var vendors = await _vendorRepo.GetByIdsAsync(vendorIds);

            return vendors;
        }

        public async Task<IList<Warehouse>> GetWarehouseByIdsAsync(IList<int> warehouseIds)
        {
            var warehouses = await _warehouseRepo.GetByIdsAsync(warehouseIds);

            return warehouses;
        }

        public async Task<bool> IsExistWarehouseAsync(int warehouseId, int? warehouseVendorId = null)
        {
            var query = _warehouseVendorRepo.Table.Where(a => a.WarehouseId == warehouseId);

            if (warehouseVendorId.HasValue)
                query = query.Where(w => w.Id != warehouseVendorId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> IsExistVendorAsync(int vendorId, int? warehouseVendorId = null)
        {
            var query = _warehouseVendorRepo.Table.Where(a => a.VendorId == vendorId);

            if (warehouseVendorId.HasValue)
                query = query.Where(w => w.Id != warehouseVendorId.Value);

            return await query.AnyAsync();
        }

        #endregion
    }
}
