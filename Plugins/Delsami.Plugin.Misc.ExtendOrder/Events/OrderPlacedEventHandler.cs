using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.Core.Events;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using ZarIran.Plugin.Misc.ExtendedOrder.Model.WarehouseLocations;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Shipping;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Events
{
    public class OrderPlacedEventHandler : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IShippingService _shippingService;
        private readonly IWarehouseLocationService _warehouseLocationService;
        private readonly IRepository<OrderShippingMethod> _repository;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderWarehouseLocationService _orderWarehouseLocationService;
        private readonly CoreSettings _coreSettings;

        #endregion

        #region Ctor

        public OrderPlacedEventHandler(IEventPublisher eventPublisher,
                                       IShippingService shippingService,
                                       IWarehouseLocationService warehouseLocationService,
                                       IRepository<OrderShippingMethod> repository,
                                       IAddressService addressService,
                                       IStateProvinceService stateProvinceService,
                                       IOrderWarehouseLocationService orderWarehouseLocationService,
                                       CoreSettings coreSettings)

        { 
            _eventPublisher = eventPublisher;
            _shippingService = shippingService;
            _warehouseLocationService = warehouseLocationService;
            _repository = repository;
            _addressService = addressService;
            _stateProvinceService = stateProvinceService;
            _orderWarehouseLocationService = orderWarehouseLocationService;
            _coreSettings = coreSettings;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            var order = eventMessage.Order;
            if (order.PaymentStatus == PaymentStatus.Paid)
                await _eventPublisher.PublishAsync(new OrderProcessingStartEvent(order));


            var shippingMethod = (await _shippingService.GetAllShippingMethodsAsync()).FirstOrDefault(a => a.Name == order.ShippingMethod);

            if (shippingMethod is not null)
            {
                var orderShippingMethod = new OrderShippingMethod()
                {
                    OrderId = order.Id,
                    ShippingMethodId = shippingMethod.Id
                };

                await _repository.InsertAsync(orderShippingMethod, false);
            }

            if (order.ShippingAddressId.HasValue)
            {
                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);

                if (shippingAddress.StateProvinceId.HasValue)
                {
                    var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(shippingAddress.StateProvinceId.Value);

                    if (stateProvince is not null)
                    {
                        var warehouseLocationRequest = new WarehouseLocationRequestModel
                        {
                            ProvinceName = stateProvince.Name,
                            CityName = shippingAddress.City
                        };

                        var warehouseLocationResponse = await _warehouseLocationService.GetOrderWarehouseFromFulfillmentAsync(warehouseLocationRequest);

                        if (warehouseLocationResponse is not null)
                        {
                            var orderWarehouseLocation = new OrderWarehouseLocation
                            {
                                Location = warehouseLocationResponse.OrderStock,
                                OrderId = order.Id
                            };

                            await _orderWarehouseLocationService.InsertAsync(orderWarehouseLocation);
                        }
                    }
                }
            }


        }

        #endregion
    }
}