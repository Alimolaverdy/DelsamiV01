using ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Models.Shipments;
using ZarIran.Plugin.Misc.ShipmentManagement.Services;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Factories
{
    public class ZarIranShipmentModelFactory : IZarIranShipmentModelFactory
    {
        #region Fields

        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly IShipmentDeliveryService _shipmentDeliveryService;

        #endregion

        #region Ctor

        public ZarIranShipmentModelFactory(IShipmentService shipmentService,
                                       IWorkContext workContext,
                                       IOrderService orderService,
                                       IDateTimeHelper dateTimeHelper,
                                       ILocalizationService localizationService,
                                       IMeasureService measureService,
                                       MeasureSettings measureSettings,
                                       IShipmentDeliveryService shipmentDeliveryService)
        {
            _shipmentService = shipmentService;
            _workContext = workContext;
            _orderService = orderService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _measureService = measureService;
            _measureSettings = measureSettings;
            _shipmentDeliveryService = shipmentDeliveryService;
        }

        #endregion

        #region Methods

        public virtual async Task<ZarIranOrderShipmentListModel> PrepareOrderShipmentListModelAsync(OrderShipmentSearchModel searchModel, Order order)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var vendor = await _workContext.GetCurrentVendorAsync();
            //get shipments
            var shipments = (await _shipmentService.GetAllShipmentsAsync(
                orderId: order.Id,
                //a vendor should have access only to his products
                vendorId: vendor?.Id ?? 0))
                .OrderBy(shipment => shipment.CreatedOnUtc)
                .ToList();

            var pagedShipments = shipments.ToPagedList(searchModel);

            //prepare list model
            var model = await new ZarIranOrderShipmentListModel().PrepareToGridAsync(searchModel, pagedShipments, () =>
            {
                //fill in model values from the entity
                return pagedShipments.SelectAwait(async shipment => await PrepareShipmentModelAsync(shipment));
            });

            return model;
        }

        #endregion

        #region Utilities

        protected virtual async Task<ZarIranShipmentModel> PrepareShipmentModelAsync(Shipment shipment, ZarIranShipmentModel model = null)
        {
            //fill in model values from the entity
            var shipmentModel = model;

            if (shipmentModel == null)
            {
                shipmentModel = shipment.ToModel<ZarIranShipmentModel>();
            }

            var shipmentDelivery = await _shipmentDeliveryService.GetShipmentDeliveryByShipmentIdAsync(shipment.Id);

            shipmentModel.Delivered = shipmentDelivery.Delivered;
            shipmentModel.DeliveryCode = shipmentDelivery.DeliveryCode;

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

            shipmentModel.PickupInStore = order.PickupInStore;
            shipmentModel.CustomOrderNumber = order.CustomOrderNumber;

            //convert dates to the user time
            if (order.PickupInStore)
            {
                shipmentModel.ShippedDate = await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DateNotAvailable");
                shipmentModel.ReadyForPickupDate = shipment.ReadyForPickupDateUtc.HasValue
                    ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ReadyForPickupDateUtc.Value, DateTimeKind.Utc)).ToString()
                    : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.ReadyForPickupDate.NotYet");
            }
            else
            {
                shipmentModel.ReadyForPickupDate = await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DateNotAvailable");
                shipmentModel.ShippedDate = shipmentDelivery.ShippedOnUtc.HasValue
                    ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipmentDelivery.ShippedOnUtc.Value, DateTimeKind.Utc)).ToString()
                    : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DateNotAvailable");
            }

            shipmentModel.DeliveryDate = shipmentDelivery.DeliveredOnUtc.HasValue
                ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipmentDelivery.DeliveredOnUtc.Value, DateTimeKind.Utc)).ToString()
                : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DeliveryDate.NotYet");

            //fill in additional values (not existing in the entity)
            shipmentModel.CanShip = !order.PickupInStore && !shipment.ShippedDateUtc.HasValue;
            shipmentModel.CanMarkAsReadyForPickup = order.PickupInStore && !shipment.ReadyForPickupDateUtc.HasValue;
            shipmentModel.CanDeliver = (shipment.ShippedDateUtc.HasValue || shipment.ReadyForPickupDateUtc.HasValue) && !shipment.DeliveryDateUtc.HasValue;

            if (shipment.TotalWeight.HasValue)
                shipmentModel.TotalWeight = $"{shipment.TotalWeight:F2} [{(await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name}]";

            return shipmentModel;
        }

        #endregion

    }
}
