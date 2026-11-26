using Delsami.Plugin.Misc.Core.Events;
using Delsami.Plugin.Misc.ShipmentManagement.Helpers;
using Delsami.Plugin.Misc.ShipmentManagement.Services;
using Delsami.Plugin.Misc.SMS;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Servicecs;

namespace Delsami.Plugin.Misc.ShipmentManagement.Events
{
    public class ShipmentCreatedConsumer : IConsumer<ShipmentCreatedEvent>
    {
        #region Fields

        private readonly IShipmentDeliveryService _shipmentDeliveryService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly ICustomerService _customerService;
        private readonly SMSSettings _smsSettings;
        private readonly ISMSSender _smsSender;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public ShipmentCreatedConsumer(IShipmentDeliveryService shipmentDeliveryService,
                                       ILocalizationService localizationService,
                                       IOrderService orderService,
                                       SMSSettings smsSettings,
                                       IShipmentService shipmentService,
                                       ICustomerService customerService,
                                       ISMSSender smsSender,
                                       IEventPublisher eventPublisher)
        {
            _shipmentDeliveryService = shipmentDeliveryService;
            _localizationService = localizationService;
            _orderService = orderService;
            _smsSettings = smsSettings;
            _shipmentService = shipmentService;
            _customerService = customerService;
            _smsSender = smsSender;
            _shipmentDeliveryService = shipmentDeliveryService;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(ShipmentCreatedEvent eventMessage)
        {
            var shipmentDelivery = await _shipmentDeliveryService.GetShipmentDeliveryByShipmentIdAsync(eventMessage.Shipment.Id);

            if (shipmentDelivery == null)
            {
                var deliveryCode = ShipmentManagementHelper.GenerateDeliveryCode();

                while (await _shipmentDeliveryService.CheckDeliveryCodeIsDuplicated(deliveryCode))
                    deliveryCode = ShipmentManagementHelper.GenerateDeliveryCode();

                shipmentDelivery = new Domain.ShipmentDelivery
                {
                    ShipmentId = eventMessage.Shipment.Id,
                    DeliveryCode = deliveryCode,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await _shipmentDeliveryService.InsertAsync(shipmentDelivery);

                var deliveryCodeGenerated = new DeliveryCodeGeneratedEvent(shipmentDelivery.ShipmentId, shipmentDelivery.DeliveryCode);

                _eventPublisher.Publish(deliveryCodeGenerated);

            }
        }

        #endregion
    }
}
