using Delsami.Plugin.Misc.ProductManagement.Domain;
using Delsami.Plugin.Widgets.AddressLocation;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Shipping;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public class WarehouseAddressCustomerDistanceService : IWarehouseAddressCustomerDistanceService
    {
        #region Fields

        private readonly IRepository<WarehouseAddressCustomerDistance> _warehouseAddressCustomerDistanceRepo;
        private readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepo;
        private readonly IRepository<Warehouse> _warehouseRepo;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly AddressLocationSettings _addressLocationSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IShippingService _shippingService;

        #endregion

        #region Ctor

        public WarehouseAddressCustomerDistanceService(IRepository<WarehouseAddressCustomerDistance> warehouseAddressCustomerDistanceRepo,
                                     ICustomerService customerService,
                                     IAddressService addressService,
                                     IAddressAttributeService addressAttributeService,
                                     AddressLocationSettings addressLocationSettings,
                                     IAddressAttributeParser addressAttributeParser,
                                     IRepository<CustomerAddressMapping> customerAddressMappingRepo,
                                     IRepository<Warehouse> warehouseRepo,
                                     IShippingService shippingService)
        {
            _warehouseAddressCustomerDistanceRepo = warehouseAddressCustomerDistanceRepo;
            _customerService = customerService;
            _addressService = addressService;
            _addressAttributeService = addressAttributeService;
            _addressLocationSettings = addressLocationSettings;
            _addressAttributeParser = addressAttributeParser;
            _customerAddressMappingRepo = customerAddressMappingRepo;
            _warehouseRepo = warehouseRepo;
            _shippingService = shippingService;
        }

        #endregion

        #region Methods

        public async Task InitializeWarehouseAddressCustomerDistancesAsync()
        {
            var warehouses = await _shippingService.GetAllWarehousesAsync();
            var latAddressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(_addressLocationSettings.LatitudeAddressAttributeId);
            var lngAddressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(_addressLocationSettings.LongitudeAddressAttributeId);

            if (latAddressAttribute == null || lngAddressAttribute == null)
                return;

            foreach (var warehouse in warehouses)
            {
                if (warehouse.AddressId != 0)
                {
                    var customers = await _customerService.GetAllCustomersAsync();

                    foreach (var customer in customers)
                    {
                        var customerAddresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);

                        foreach (var customerAddress in customerAddresses)
                        {
                            if (!string.IsNullOrEmpty(customerAddress.CustomAttributes))
                            {
                                var customerAddressLatValue = _addressAttributeParser.ParseValues(customerAddress.CustomAttributes, latAddressAttribute.Id)[0];
                                var customerAddressLngValue = _addressAttributeParser.ParseValues(customerAddress.CustomAttributes, lngAddressAttribute.Id)[0];

                                if (string.IsNullOrWhiteSpace(customerAddressLatValue) || string.IsNullOrWhiteSpace(customerAddressLngValue))
                                    continue;

                                var warehouseAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);

                                if (warehouseAddress == null)
                                    continue;

                                if (!string.IsNullOrWhiteSpace(warehouseAddress.CustomAttributes))
                                {
                                    var vendorAddressLatValue = _addressAttributeParser.ParseValues(warehouseAddress.CustomAttributes, latAddressAttribute.Id)[0];
                                    var vendorAddressLngValue = _addressAttributeParser.ParseValues(warehouseAddress.CustomAttributes, lngAddressAttribute.Id)[0];

                                    var warehouseAddressCustomerDistance = new WarehouseAddressCustomerDistance
                                    {
                                        CustomerAddressId = customerAddress.Id,
                                        WarehouseAddressId = warehouse.AddressId,
                                        Distance = (int)GetDistance(customerAddressLatValue, customerAddressLngValue, vendorAddressLatValue, vendorAddressLngValue)
                                    };

                                    await _warehouseAddressCustomerDistanceRepo.InsertAsync(warehouseAddressCustomerDistance);
                                }
                            }
                        }
                    }
                }
            }
        }

        public async Task DeleteWarehouseAddressCustomerDistanceByCustomerAddressIdAsync(int customerAddressId)
        {
            if (customerAddressId == 0)
                return;

            await _warehouseAddressCustomerDistanceRepo.DeleteAsync(x => x.CustomerAddressId == customerAddressId);
        }

        public async Task InsertOrUpdateByAddressIdAsync(int addressId, bool? isCustomerAddress = null, bool? isWarehouseAddress = null)
        {
            if (addressId == 0)
                return;

            var latAddressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(_addressLocationSettings.LatitudeAddressAttributeId);
            var lngAddressAttribute = await _addressAttributeService.GetAddressAttributeByIdAsync(_addressLocationSettings.LongitudeAddressAttributeId);

            if (latAddressAttribute == null || lngAddressAttribute == null)
                return;

            if (!isCustomerAddress.HasValue && !isWarehouseAddress.HasValue)
            {
                isCustomerAddress = await _customerAddressMappingRepo.Table.AnyAsync(a => a.AddressId == addressId);
                isWarehouseAddress = await _warehouseRepo.Table.AnyAsync(a => a.AddressId == addressId);
            }

            if (isCustomerAddress == true)
            {
                await _warehouseAddressCustomerDistanceRepo.DeleteAsync(x => x.CustomerAddressId == addressId);

                var warehouses = await _shippingService.GetAllWarehousesAsync();

                if (latAddressAttribute == null || lngAddressAttribute == null)
                    return;

                var address = await _addressService.GetAddressByIdAsync(addressId);

                foreach (var warehouse in warehouses)
                {
                    if (warehouse.AddressId != 0)
                    {
                        if (!string.IsNullOrEmpty(address.CustomAttributes))
                        {
                            var customerAddressLatValue = _addressAttributeParser.ParseValues(address.CustomAttributes, latAddressAttribute.Id)[0];
                            var customerAddressLngValue = _addressAttributeParser.ParseValues(address.CustomAttributes, lngAddressAttribute.Id)[0];

                            if (string.IsNullOrWhiteSpace(customerAddressLatValue) || string.IsNullOrWhiteSpace(customerAddressLngValue))
                                continue;

                            var customerAddress = await _addressService.GetAddressByIdAsync(warehouse.AddressId);

                            if (customerAddress == null)
                                continue;

                            var warehouseAddressLatValue = _addressAttributeParser.ParseValues(customerAddress.CustomAttributes, latAddressAttribute.Id)[0];
                            var warehouseAddressLngValue = _addressAttributeParser.ParseValues(customerAddress.CustomAttributes, lngAddressAttribute.Id)[0];

                            var warehouseAddressCustomerDistance = new WarehouseAddressCustomerDistance
                            {
                                CustomerAddressId = address.Id,
                                WarehouseAddressId = warehouse.AddressId,
                                Distance = (int)GetDistance(customerAddressLatValue, customerAddressLngValue, warehouseAddressLatValue, warehouseAddressLngValue)
                            };

                            await _warehouseAddressCustomerDistanceRepo.InsertAsync(warehouseAddressCustomerDistance);
                        }
                    }
                }
            }
            else if (isWarehouseAddress == true)
            {
                await _warehouseAddressCustomerDistanceRepo.DeleteAsync(x => x.WarehouseAddressId == addressId);

                var warehouseAddress = await _addressService.GetAddressByIdAsync(addressId);

                if (warehouseAddress == null || string.IsNullOrWhiteSpace(warehouseAddress.CustomAttributes))
                    return;

                var vendorAddressLatValue = _addressAttributeParser.ParseValues(warehouseAddress.CustomAttributes, latAddressAttribute.Id)[0];
                var vendorAddressLngValue = _addressAttributeParser.ParseValues(warehouseAddress.CustomAttributes, lngAddressAttribute.Id)[0];

                if (string.IsNullOrWhiteSpace(vendorAddressLatValue) || string.IsNullOrWhiteSpace(vendorAddressLngValue))
                    return;

                var customers = await _customerService.GetAllCustomersAsync();

                foreach (var customer in customers)
                {
                    var customerAddresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);

                    foreach (var customerAddress in customerAddresses)
                    {
                        if (!string.IsNullOrEmpty(customerAddress.CustomAttributes))
                        {
                            var customerAddressLatValue = _addressAttributeParser.ParseValues(customerAddress.CustomAttributes, latAddressAttribute.Id)[0];
                            var customerAddressLngValue = _addressAttributeParser.ParseValues(customerAddress.CustomAttributes, lngAddressAttribute.Id)[0];

                            if (string.IsNullOrWhiteSpace(customerAddressLatValue) || string.IsNullOrWhiteSpace(customerAddressLngValue))
                                continue;

                            var warehouseAddressCustomerDistance = new WarehouseAddressCustomerDistance
                            {
                                CustomerAddressId = customerAddress.Id,
                                WarehouseAddressId = addressId,
                                Distance = (int)GetDistance(customerAddressLatValue, customerAddressLngValue, vendorAddressLatValue, vendorAddressLngValue)
                            };

                            await _warehouseAddressCustomerDistanceRepo.InsertAsync(warehouseAddressCustomerDistance);
                        }
                    }
                }
            }
        }

        public async Task DeleteWarehouseAddressCustomerDistanceByWarehouseAddressIdAsync(int warehouseAddressId)
        {
            if (warehouseAddressId == 0)
                return;

            await _warehouseAddressCustomerDistanceRepo.DeleteAsync(x => x.WarehouseAddressId == warehouseAddressId);
        }

        public async Task<int?> GetDistanceAsync(int customerId, int warehouseId)
        {
            if (warehouseId == 0)
                return null;

            if (customerId == 0)
                return null;

            var customerAddress = (await _customerService.GetAddressesByCustomerIdAsync(customerId)).FirstOrDefault();

            if (customerAddress == null)
                return null;

            var warehouse = await _shippingService.GetWarehouseByIdAsync(warehouseId);

            if (warehouse == null || warehouse.AddressId == 0)
                return null;

            var warehouseAddressCustomerDistance = await _warehouseAddressCustomerDistanceRepo.Table.FirstOrDefaultAsync(w => w.CustomerAddressId == customerAddress.Id && w.WarehouseAddressId == warehouse.AddressId);

            return warehouseAddressCustomerDistance?.Distance;
        }

        #endregion

        #region Utilties

        private double GetDistance(string lat1Str, string lng1Str, string lat2Str, string lng2Str)
        {
            var lat1 = decimal.Parse(lat1Str, NumberStyles.Number, CultureInfo.InvariantCulture);
            var lng1 = decimal.Parse(lng1Str, NumberStyles.Number, CultureInfo.InvariantCulture);
            var lat2 = decimal.Parse(lat2Str, NumberStyles.Number, CultureInfo.InvariantCulture);
            var lng2 = decimal.Parse(lng2Str, NumberStyles.Number, CultureInfo.InvariantCulture);

            var d1 = (double)lat1 * (Math.PI / 180.0);
            var num1 = (double)lng1 * (Math.PI / 180.0);
            var d2 = (double)lat2 * (Math.PI / 180.0);
            var num2 = (double)lng2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        #endregion
    }
}
