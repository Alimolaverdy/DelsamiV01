using System.Threading.Tasks;
using Delsami.Plugin.Misc.ProductManagement.Services;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Delsami.Plugin.Misc.ProductManagement.EventHandlers
{
    public class CustomerAddressEventHandler : IConsumer<EntityDeletedEvent<CustomerAddressMapping>>,
                                               IConsumer<EntityUpdatedEvent<Address>>,
                                               IConsumer<EntityInsertedEvent<CustomerAddressMapping>>,
                                               IConsumer<EntityInsertedEvent<Warehouse>>,
                                               IConsumer<EntityUpdatedEvent<Warehouse>>,
                                               IConsumer<EntityDeletedEvent<Warehouse>>,
                                               IConsumer<EntityDeletedEvent<Vendor>>

    {
        #region Fields

        private readonly IWarehouseAddressCustomerDistanceService _warehouseAddressCustomerDistanceService;
        private readonly IWarehouseVendorService _warehouseVendorService;

        #endregion

        #region Ctor

        public CustomerAddressEventHandler(IWarehouseAddressCustomerDistanceService warehouseAddressCustomerDistanceService,
                                           IWarehouseVendorService warehouseVendorService)
        {
            _warehouseAddressCustomerDistanceService = warehouseAddressCustomerDistanceService;
            _warehouseVendorService = warehouseVendorService;
        }

        #endregion

        #region Hanlders

        //Update customer address in admin area
        public async Task HandleEventAsync(EntityUpdatedEvent<Address> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            await _warehouseAddressCustomerDistanceService.InsertOrUpdateByAddressIdAsync(eventMessage.Entity.Id);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<CustomerAddressMapping> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            await _warehouseAddressCustomerDistanceService.DeleteWarehouseAddressCustomerDistanceByCustomerAddressIdAsync(eventMessage.Entity.AddressId);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Warehouse> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            await _warehouseAddressCustomerDistanceService.DeleteWarehouseAddressCustomerDistanceByWarehouseAddressIdAsync(eventMessage.Entity.AddressId);
            await _warehouseVendorService.DeleteByWarehouseIdAsync(eventMessage.Entity.Id);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            await _warehouseVendorService.DeleteByVendorIdAsync(eventMessage.Entity.Id);
        }

        //insert customer address in admin area
        public async Task HandleEventAsync(EntityInsertedEvent<CustomerAddressMapping> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            await _warehouseAddressCustomerDistanceService.InsertOrUpdateByAddressIdAsync(eventMessage.Entity.AddressId, isCustomerAddress: true);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Warehouse> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            await _warehouseAddressCustomerDistanceService.InsertOrUpdateByAddressIdAsync(eventMessage.Entity.AddressId, isWarehouseAddress: true);
        }

        public async Task HandleEventAsync(EntityInsertedEvent<Warehouse> eventMessage)
        {
            await _warehouseAddressCustomerDistanceService.InsertOrUpdateByAddressIdAsync(eventMessage.Entity.AddressId, isWarehouseAddress: true);
        }

        #endregion
    }
}
