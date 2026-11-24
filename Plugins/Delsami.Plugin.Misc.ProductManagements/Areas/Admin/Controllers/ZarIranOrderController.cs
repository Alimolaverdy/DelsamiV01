using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Nop.Services.Orders;
using Nop.Web.Areas.Admin.Factories;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using Nop.Core.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Core.Domain.Orders;
using Nop.Services.Logging;
using ZarIran.Plugin.Misc.ProductManagements.Services;
using ZarIran.Plugin.Misc.Core.Services;

namespace ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Controllers
{
    public class ZarIranOrderController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IProductService _productService;
        private readonly IShipmentService _shipmentService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVirtualProductMappingService _virtualProductMappingService;

        #endregion

        #region Ctor

        public ZarIranOrderController(IPermissionService permissionService,
                                       IOrderService orderService,
                                       IWorkContext workContext,
                                       IOrderModelFactory orderModelFactory,
                                       IProductService productService,
                                       IShipmentService shipmentService,
                                       IEventPublisher eventPublisher,
                                       IOrderProcessingService orderProcessingService,
                                       ILocalizationService localizationService,
                                       INotificationService notificationService,
                                       ICustomerActivityService customerActivityService,
                                       IVirtualProductMappingService virtualProductMappingService)
        {
            _permissionService = permissionService;
            _orderService = orderService;
            _workContext = workContext;
            _orderModelFactory = orderModelFactory;
            _productService = productService;
            _shipmentService = shipmentService;
            _eventPublisher = eventPublisher;
            _orderProcessingService = orderProcessingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _customerActivityService = customerActivityService;
            _virtualProductMappingService = virtualProductMappingService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> AddShipment(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
                return RedirectToAction("List");

            //prepare model
            var model = await _orderModelFactory.PrepareShipmentModelAsync(new ShipmentModel(), null, order);

            return View("~/Plugins/ZarIran.Misc.ProductManagements/Areas/Admin/Views/ZarIranOrder/AddShipment.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> AddShipment(ShipmentModel model, IFormCollection form, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && !await HasAccessToOrderAsync(order))
                return RedirectToAction("List");

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);
            //a vendor should have access only to his products
            if (currentVendor != null)
            {
                orderItems = await orderItems.WhereAwait(HasAccessToProductAsync).ToListAsync();
            }

            var shipment = new Shipment
            {
                OrderId = order.Id,
                TrackingNumber = model.TrackingNumber,
                TotalWeight = null,
                AdminComment = model.AdminComment,
                CreatedOnUtc = DateTime.UtcNow
            };

            var shipmentItems = new List<ShipmentItem>();

            decimal? totalWeight = null;

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                var mainProductResult = (await _virtualProductMappingService.GetMainProductByVirtualProductIdAsync(product.Id));

                //ensure that this product can be shipped (have at least one item to ship)
                var maxQtyToAdd = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                if (maxQtyToAdd <= 0)
                    continue;

                var qtyToAdd = 0; //parse quantity
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(form[formKey], out qtyToAdd);
                        break;
                    }

                var warehouseId = 0;

                if (mainProductResult.MainProduct != null)
                    warehouseId = GetWarehouseId(mainProductResult.MainProduct, form, orderItem.Id);
                else
                    warehouseId = GetWarehouseId(product, form, orderItem.Id);

                //if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                //    product.UseMultipleWarehouses)
                //{
                //    //multiple warehouses supported
                //    //warehouse is chosen by a store owner
                //    foreach (var formKey in form.Keys)
                //        if (formKey.Equals($"warehouse_{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                //        {
                //            _ = int.TryParse(form[formKey], out warehouseId);
                //            break;
                //        }
                //}
                //else
                //{
                //    //multiple warehouses are not supported
                //    warehouseId = product.WarehouseId;
                //}

                //validate quantity
                if (qtyToAdd <= 0)
                    continue;
                if (qtyToAdd > maxQtyToAdd)
                    qtyToAdd = maxQtyToAdd;

                //ok. we have at least one item. let's create a shipment (if it does not exist)

                var orderItemTotalWeight = orderItem.ItemWeight * qtyToAdd;
                if (orderItemTotalWeight.HasValue)
                {
                    if (!totalWeight.HasValue)
                        totalWeight = 0;
                    totalWeight += orderItemTotalWeight.Value;
                }

                //create a shipment item
                shipmentItems.Add(new ShipmentItem
                {
                    OrderItemId = orderItem.Id,
                    Quantity = qtyToAdd,
                    WarehouseId = warehouseId
                });
            }

            //if we have at least one item in the shipment, then save it
            if (shipmentItems.Any())
            {
                shipment.TotalWeight = totalWeight;
                await _shipmentService.InsertShipmentAsync(shipment);

                foreach (var shipmentItem in shipmentItems)
                {
                    shipmentItem.ShipmentId = shipment.Id;
                    await _shipmentService.InsertShipmentItemAsync(shipmentItem);
                }

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "A shipment has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await _eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));

                var canShip = !order.PickupInStore && model.CanShip;
                if (canShip)
                    await _orderProcessingService.ShipAsync(shipment, true);

                var canMarkAsReadyForPickup = order.PickupInStore && model.CanMarkAsReadyForPickup;
                if (canMarkAsReadyForPickup)
                    await _orderProcessingService.ReadyForPickupAsync(shipment, true);

                if ((canShip || canMarkAsReadyForPickup) && model.CanDeliver)
                    await _orderProcessingService.DeliverAsync(shipment, true);

                await LogEditOrderAsync(order.Id);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.Added"));
                return continueEditing
                        ? RedirectToAction("ShipmentDetails", "Order", new { id = shipment.Id })
                        : RedirectToAction("Edit", "Order", new { id = model.OrderId });
            }

            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.NoProductsSelected"));

            return RedirectToAction("AddShipment", model);
        }

        #endregion

        #region Utilities

        protected virtual async ValueTask<bool> HasAccessToOrderAsync(Order order)
        {
            return order != null && await HasAccessToOrderAsync(order.Id);
        }

        protected virtual async Task<bool> HasAccessToOrderAsync(int orderId)
        {
            if (orderId == 0)
                return false;

            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = currentVendor.Id;
            var hasVendorProducts = (await _orderService.GetOrderItemsAsync(orderId, vendorId: vendorId)).Any();

            return hasVendorProducts;
        }

        protected virtual async Task LogEditOrderAsync(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            await _customerActivityService.InsertActivityAsync("EditOrder",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
        }

        protected virtual async ValueTask<bool> HasAccessToProductAsync(OrderItem orderItem)
        {
            if (orderItem == null || orderItem.ProductId == 0)
                return false;

            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = currentVendor.Id;

            return (await _productService.GetProductByIdAsync(orderItem.ProductId))?.VendorId == vendorId;
        }

        public int GetWarehouseId(Product product, IFormCollection form, int orderItemId)
        {
            var warehouseId = 0;

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.UseMultipleWarehouses)
            {
                //multiple warehouses supported
                //warehouse is chosen by a store owner
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"warehouse_{orderItemId}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(form[formKey], out warehouseId);
                        break;
                    }
            }
            else
            {
                //multiple warehouses are not supported
                warehouseId = product.WarehouseId;
            }

            return warehouseId;
        }
        #endregion
    }
}
