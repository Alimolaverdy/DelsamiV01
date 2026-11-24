using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.Core.Events;
using ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Factories;
using ZarIran.Plugin.Misc.ShipmentManagement.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System; 
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class ShipmentManagementOrderController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IShipmentTrackingImportService _importService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IZarIranShipmentModelFactory _ZarIranShipmentModelFactory;
        private readonly IShipmentService _shipmentService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly INotificationService _notificationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public ShipmentManagementOrderController(IPermissionService permissionService,
                                                 IOrderService orderService,
                                                 IShipmentTrackingImportService importService,
                                                 IWorkContext workContext,
                                                 IZarIranShipmentModelFactory ZarIranShipmentModelFactory,
                                                 IShipmentService shipmentService,
                                                 IOrderModelFactory orderModelFactory,
                                                 INotificationService notificationService,
                                                 IOrderProcessingService orderProcessingService,
                                                 ICustomerActivityService customerActivityService,
                                                 ILocalizationService localizationService,
                                                 IEventPublisher eventPublisher,
                                                 IProductService productService)
        {
            _permissionService = permissionService;
            _orderService = orderService;
            _workContext = workContext;
            _importService = importService;
            _ZarIranShipmentModelFactory = ZarIranShipmentModelFactory;
            _shipmentService = shipmentService;
            _orderModelFactory = orderModelFactory;
            _notificationService = notificationService;
            _orderProcessingService = orderProcessingService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
            _productService = productService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual async Task<IActionResult> ShipmentsByOrder(OrderShipmentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(searchModel.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
                return Content(string.Empty);

            //prepare model
            var model = await _ZarIranShipmentModelFactory.PrepareOrderShipmentListModelAsync(searchModel, order);

            return Json(model);
        }

        public virtual async Task<IActionResult> ShipmentDetails(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            //prepare model
            var model = await _orderModelFactory.PrepareShipmentModelAsync(null, shipment, null);

            return View("~/Plugins/ZarIran.Misc.CustomPdfGenerator/Areas/Admin/Views/CustomOrder/ShipmentDetails.cshtml", model);
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("settrackingnumber")]
        public virtual async Task<IActionResult> SetTrackingNumber(ShipmentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            var sendSms = false;

            if (string.IsNullOrWhiteSpace(shipment.TrackingNumber) && !string.IsNullOrWhiteSpace(model.TrackingNumber))
                sendSms = true;

            shipment.TrackingNumber = model.TrackingNumber;
            await _shipmentService.UpdateShipmentAsync(shipment);

            var trackingNumberSetEvent = new TrackingNumberSetEvent
            {
                Shipment = shipment,
                SendSMS = sendSms,
            };

            await _eventPublisher.PublishAsync(trackingNumberSetEvent);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setadmincomment")]
        public virtual async Task<IActionResult> SetShipmentAdminComment(ShipmentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            shipment.AdminComment = model.AdminComment;
            await _shipmentService.UpdateShipmentAsync(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasshipped")]
        public virtual async Task<IActionResult> SetAsShipped(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                await _orderProcessingService.ShipAsync(shipment, true);
                await LogEditOrderAsync(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("saveshippeddate")]
        public virtual async Task<IActionResult> EditShippedDate(ShipmentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.ShippedDateUtc.HasValue)
                {
                    throw new Exception("Enter shipped date");
                }

                shipment.ShippedDateUtc = model.ShippedDateUtc;
                await _shipmentService.UpdateShipmentAsync(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasreadyforpickup")]
        public virtual async Task<IActionResult> SetAsReadyForPickup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                await _orderProcessingService.ReadyForPickupAsync(shipment, true);
                await LogEditOrderAsync(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("savereadyforpickupdate")]
        public virtual async Task<IActionResult> EditReadyForPickupDate(ShipmentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.ReadyForPickupDateUtc.HasValue)
                    throw new Exception("Enter ready for pickup date");

                shipment.ReadyForPickupDateUtc = model.ReadyForPickupDateUtc;
                await _shipmentService.UpdateShipmentAsync(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasdelivered")]
        public virtual async Task<IActionResult> SetAsDelivered(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                await _orderProcessingService.DeliverAsync(shipment, true);
                await LogEditOrderAsync(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("savedeliverydate")]
        public virtual async Task<IActionResult> EditDeliveryDate(ShipmentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.DeliveryDateUtc.HasValue)
                {
                    throw new Exception("Enter delivery date");
                }

                shipment.DeliveryDateUtc = model.DeliveryDateUtc;
                await _shipmentService.UpdateShipmentAsync(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteShipment(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToShipmentAsync(shipment))
                return RedirectToAction("List");

            foreach (var shipmentItem in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                await _productService.ReverseBookedInventoryAsync(product, shipmentItem,
                    string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteShipment"), shipment.OrderId));
            }

            var orderId = shipment.OrderId;
            await _shipmentService.DeleteShipmentAsync(shipment);

            var order = await _orderService.GetOrderByIdAsync(orderId);
            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "A shipment has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Orders.Shipments.Deleted"));
            return RedirectToAction("Edit", "Order", new { id = orderId });
        }










        //public virtual async Task<IActionResult> ImportTrackingNumbers()
        //{
        //    if (!await _permissionService.AuthorizeAsync("ManageShipmentManagement"))
        //        return AccessDeniedView();

        //    return View("/Areas/Admin/Views/ShipmentTrackNumberImport/ImportTrackingNumbers.cshtml");
        //}

        //[HttpPost]
        //public virtual async Task<IActionResult> ImportTrackingNumbers(IFormFile uploadFile)
        //{
        //    if (!await _permissionService.AuthorizeAsync("ManageShipmentManagement"))
        //        return AccessDeniedView();

        //    if (uploadFile == null || uploadFile.Length == 0)
        //    {
        //        _notificationService.ErrorNotification("فایل انتخاب نشده");
        //        return View("/Areas/Admin/Views/ShipmentTrackNumberImport/ImportTrackingNumbers.cshtml");
        //    }

        //    byte[] fileBytes;
        //    using (var stream = new MemoryStream())
        //    {
        //        await uploadFile.CopyToAsync(stream);
        //        fileBytes = stream.ToArray();
        //    }

        //    var user = await _workContext.GetCurrentCustomerAsync();
        //    try
        //    {
        //        int importId = await _importService.ImportTrackingNumbersAsync(fileBytes, user.Id);
        //        _notificationService.SuccessNotification("ورود فایل با موفقیت انجام شد.");
        //        return RedirectToAction("ImportReport", new { id = importId });
        //    }
        //    catch (Exception ex)
        //    {
        //        _notificationService.ErrorNotification(ex.Message);
        //        return View("/Areas/Admin/Views/ShipmentTrackNumberImport/ImportTrackingNumbers.cshtml");
        //    }
        //}

        //public virtual async Task<IActionResult> ImportReports()
        //{
        //    if (!await _permissionService.AuthorizeAsync("ManageShipmentManagement"))
        //        return AccessDeniedView();

        //    var imports = await _importService.GetImportsAsync();
        //    return View("/Areas/Admin/Views/ShipmentTrackNumberImport/ImportReports.cshtml", imports);
        //}

        //public virtual async Task<IActionResult> ImportReport(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync("ManageShipmentManagement"))
        //        return AccessDeniedView();

        //    var reports = await _importService.GetReportsByImportIdAsync(id);
        //    return View("/Areas/Admin/Views/ShipmentTrackNumberImport/ImportReport.cshtml", reports);
        //}


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

        protected virtual async ValueTask<bool> HasAccessToShipmentAsync(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (await _workContext.GetCurrentVendorAsync() == null)
                //not a vendor; has access
                return true;

            return await HasAccessToOrderAsync(shipment.OrderId);
        }

        protected virtual async Task LogEditOrderAsync(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            await _customerActivityService.InsertActivityAsync("EditOrder",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
        }
        #endregion
    }
}
