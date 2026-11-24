using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Models.WarehouseVendors;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using ZarIran.Plugin.Misc.ProductManagements.Services;
using Nop.Core;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Factories
{
    public class WarehouseVendorFactory : IWarehouseVendorFactory
    {
        #region Fields

        private readonly IWarehouseVendorService _warehouseVendorService;
        private readonly IWorkContext _workContext;
        private readonly IVendorService _vendorService;
        private readonly IShippingService _shippingService;

        #endregion

        #region Ctor

        public WarehouseVendorFactory(IWarehouseVendorService warehouseVendorService,
                                      IWorkContext workContext,
                                      IVendorService vendorService,
                                      IShippingService shippingService)
        {
            _warehouseVendorService = warehouseVendorService;
            _workContext = workContext;
            _vendorService = vendorService;
            _shippingService = shippingService;
        }

        #endregion

        #region Methods

        public virtual async Task<WarehouseVendorSearchModel> PrepareWarehouseVendorSearchModelAsync(WarehouseVendorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);

        }

        public virtual async Task<WarehouseVendorListModel> PrepareWarehouseVendorListModelAsync(WarehouseVendorSearchModel searchModel, int? customerId = null)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var warehouseVendors = await _warehouseVendorService.GetWarehouseVendorsAsync(pageIndex: searchModel.Page - 1,
                                                                                   pageSize: searchModel.PageSize);

            var warehouses = await _warehouseVendorService.GetWarehouseByIdsAsync(warehouseVendors.Select(s => s.WarehouseId).ToList());
            var vendors = await _warehouseVendorService.GetVendorByIdsAsync(warehouseVendors.Select(s => s.VendorId).ToList());

            //prepare grid model
            var model = new WarehouseVendorListModel().PrepareToGrid(searchModel, warehouseVendors, () =>
            {
                return warehouseVendors.Select( warehouseVendor =>
                {
                    //fill in model values from the entity
                    var warehouseVendorModel = warehouseVendor.ToModel<WarehouseVendorModel>();

                    warehouseVendorModel.WarehouseName = warehouses.FirstOrDefault(f => f.Id == warehouseVendor.WarehouseId)?.Name;
                    warehouseVendorModel.VendorName = vendors.FirstOrDefault(f => f.Id == warehouseVendor.VendorId)?.Name;

                    return warehouseVendorModel;
                });
            });

            return model;
        }

        public async Task<WarehouseVendorModel> PrepareWarehouseVendorModelAsync(WarehouseVendorModel warehouseVendorModel, WarehouseVendor warehouseVendor)
        {
            if (warehouseVendorModel == null)
            {
                warehouseVendorModel = new WarehouseVendorModel();

                if (warehouseVendor != null)
                    warehouseVendorModel = warehouseVendor.ToModel<WarehouseVendorModel>();
            }

            //warehouseVendorModel.ModelSpecificationAttributeId = _carInfoSettings.ModelSpecificationAttributeId;
            //warehouseVendorModel.YearSpecificationAttributeId = _carInfoSettings.YearSpecificationAttributeId;

            var vendors = await _vendorService.GetAllVendorsAsync();

            warehouseVendorModel.AvailableVendors = vendors.Select(vendor => new SelectListItem
            {
                Text = vendor.Name,
                Value = vendor.Id.ToString()
            }).ToList();

            var warehouses = await _shippingService.GetAllWarehousesAsync();

            warehouseVendorModel.AvailableWarehouses = warehouses.Select(warehouse => new SelectListItem
            {
                Text = warehouse.Name,
                Value = warehouse.Id.ToString()
            }).ToList();

            return warehouseVendorModel;
        }

        #endregion
    }
}
