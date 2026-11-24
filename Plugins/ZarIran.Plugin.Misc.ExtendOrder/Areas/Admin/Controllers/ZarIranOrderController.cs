using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZarIran.Plugin.Misc.Core.Events.Audit.Entities;
using ZarIran.Plugin.Misc.Core.Extensions;
using ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories;
using ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class ZarIranOrderController : BasePluginController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IZarIranOrderModelFactory _ZarIranOrderModelFactory;
        private readonly IOrderService _orderService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISubStatusMappingService _subStatusMappingService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly INotificationService _notificationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductService _productService;
        private readonly IPdfService _pdfService;
        private readonly OrderSettings _orderSettings;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IEncryptionService _encryptionService;
        private readonly IPaymentService _paymentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IShippingService _shippingService;
        private readonly IGiftCardService _giftCardService;
        private readonly IAddressService _addressService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IAddressAttributeParser _addressAttributeParser;
        #endregion

        #region Ctor
        public ZarIranOrderController(IPermissionService permissionService,
                                       IZarIranOrderModelFactory ZarIranOrderModelFactory,
                                       IOrderService orderService,
                                       IOrderModelFactory orderModelFactory,
                                       ICustomerService customerService,
                                       IWorkContext workContext,
                                       IShippingService shippingService,
                                       IStoreContext storeContext,
                                       IShoppingCartService shoppingCartService,
                                       ISubStatusMappingService subStatusMappingService,
                                       IOrderProcessingService orderProcessingService,
                                       ICustomerActivityService customerActivityService,
                                       INotificationService notificationService,
                                       IEventPublisher eventPublisher,
                                       ILocalizationService localizationService,
                                       IDateTimeHelper dateTimeHelper,
                                       IProductService productService,
                                       IPdfService pdfService,
                                       OrderSettings orderSettings,
                                       IProductAttributeParser productAttributeParser,
                                       IProductAttributeService productAttributeService,
                                       IEncryptionService encryptionService,
                                       IPaymentService paymentService,
                                       IGiftCardService giftCardService,
                                       IAddressService addressService,
                                       IPriceCalculationService priceCalculationService,
                                       IProductAttributeFormatter productAttributeFormatter,
                                       IAddressAttributeParser addressAttributeParser)
        {
            _permissionService = permissionService;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
            _ZarIranOrderModelFactory = ZarIranOrderModelFactory;
            _orderService = orderService;
            _shippingService = shippingService;
            _orderModelFactory = orderModelFactory;
            _workContext = workContext;
            _subStatusMappingService = subStatusMappingService;
            _orderProcessingService = orderProcessingService;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _productService = productService;
            _pdfService = pdfService;
            _storeContext = storeContext;
            _orderSettings = orderSettings;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _encryptionService = encryptionService;
            _paymentService = paymentService;
            _giftCardService = giftCardService;
            _notificationService = notificationService;
            _priceCalculationService = priceCalculationService;
            _addressService = addressService;
            _addressAttributeParser = addressAttributeParser;
            _productAttributeFormatter = productAttributeFormatter;
        }

        #endregion

        #region Methods

        #region List
        public virtual async Task<IActionResult> List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await _ZarIranOrderModelFactory.PrepareOrderSearchModelAsync(new ZarIranOrderSearchModel
            {
                OrderStatusIds = orderStatuses,
                PaymentStatusIds = paymentStatuses,
                ShippingStatusIds = shippingStatuses
            });

            return View("~/Plugins/ZarIran.Misc.ExtendedOrder/Areas/Admin/Views/ExtendedOrder/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OrderList(ZarIranOrderSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _ZarIranOrderModelFactory.PrepareOrderListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportAggregates(OrderSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _ZarIranOrderModelFactory.PrepareOrderAggregatorModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-order-by-number")]
        public virtual async Task<IActionResult> GoToOrderId(OrderSearchModel model)
        {
            var order = await _orderService.GetOrderByCustomOrderNumberAsync(model.GoDirectlyToCustomOrderNumber);

            if (order == null)
                return await List();

            return RedirectToAction("Edit", "Order", new { id = order.Id, area = "Admin" });
        }

        #endregion

        #region Edit

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.Deleted)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
                return RedirectToAction("List");

            //prepare model
            var model = await _ZarIranOrderModelFactory.PrepareOrderModelAsync(null, order);

            return View("~/Plugins/ZarIran.Misc.ExtendedOrder/Areas/Admin/Views/ExtendedOrder/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSubStatus(int orderId, int subStatusId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            var substatus = new OrderSubStatusMapping
            {
                OrderId = orderId,
                SubStatusId = subStatusId,
                CreatedDateTime = DateTime.UtcNow,
            };
            await _subStatusMappingService.InsertAsync(substatus);

            return RedirectToAction("Edit", new { id = orderId });
        }

        #endregion

        #region Order details

        #region Add Order Item
        [HttpPost]
        public virtual async Task<IActionResult> AddProductToOrderDetails(int orderId, int productId, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //basic properties
            _ = decimal.TryParse(form["UnitPriceInclTax"], out var unitPriceInclTax);
            _ = decimal.TryParse(form["UnitPriceExclTax"], out var unitPriceExclTax);
            _ = int.TryParse(form["Quantity"], out var quantity);
            _ = decimal.TryParse(form["SubTotalInclTax"], out var priceInclTax);
            _ = decimal.TryParse(form["SubTotalExclTax"], out var priceExclTax);

            //warnings
            var warnings = new List<string>();

            //attributes
            var attributesXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, warnings);

            //rental product
            _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

            //warnings
            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(customer, ShoppingCartType.ShoppingCart, product, quantity, attributesXml));
            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType.ShoppingCart, product, attributesXml));
            warnings.AddRange(await _shoppingCartService.GetRentalProductWarningsAsync(product, rentalStartDate, rentalEndDate));
            if (!warnings.Any())
            {
                //no errors
                var currentStore = await _storeContext.GetCurrentStoreAsync();

                //attributes
                var attributeDescription = await _productAttributeFormatter.FormatAttributesAsync(product, attributesXml, customer, currentStore);

                //weight
                var itemWeight = await _shippingService.GetShoppingCartItemWeightAsync(product, attributesXml);

                //save item
                var orderItem = new OrderItem
                {
                    OrderItemGuid = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    UnitPriceInclTax = unitPriceInclTax,
                    UnitPriceExclTax = unitPriceExclTax,
                    PriceInclTax = priceInclTax,
                    PriceExclTax = priceExclTax,
                    OriginalProductCost = await _priceCalculationService.GetProductCostAsync(product, attributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = attributesXml,
                    Quantity = quantity,
                    DiscountAmountInclTax = decimal.Zero,
                    DiscountAmountExclTax = decimal.Zero,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    ItemWeight = itemWeight,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate
                };

                await _orderService.InsertOrderItemAsync(orderItem);

                await _eventPublisher.PublishAsync(new AuditCreatedEntityEvent<OrderItem>(orderItem, order.Id.ToString()));

                //adjust inventory
                await _productService.AdjustInventoryAsync(product, -orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
                {
                    PriceInclTax = unitPriceInclTax,
                    PriceExclTax = unitPriceExclTax,
                    SubTotalInclTax = priceInclTax,
                    SubTotalExclTax = priceExclTax,
                    Quantity = quantity
                };
                await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "A new order item has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await LogEditOrderAsync(order.Id);

                //gift cards
                if (product.IsGiftCard)
                {
                    _productAttributeParser.GetGiftCardAttribute(
                        attributesXml, out var recipientName, out var recipientEmail, out var senderName, out var senderEmail, out var giftCardMessage);

                    for (var i = 0; i < orderItem.Quantity; i++)
                    {
                        var gc = new GiftCard
                        {
                            GiftCardType = product.GiftCardType,
                            PurchasedWithOrderItemId = orderItem.Id,
                            Amount = unitPriceExclTax,
                            IsGiftCardActivated = false,
                            GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                            RecipientName = recipientName,
                            RecipientEmail = recipientEmail,
                            SenderName = senderName,
                            SenderEmail = senderEmail,
                            Message = giftCardMessage,
                            IsRecipientNotified = false,
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        await _giftCardService.InsertGiftCardAsync(gc);
                    }
                }

                //redirect to order details page
                foreach (var warning in updateOrderParameters.Warnings)
                    _notificationService.WarningNotification(warning);

                //selected card
                SaveSelectedCardName("order-products");
                return RedirectToAction("Edit", new { id = order.Id });
            }

            //prepare model
            var model = await _orderModelFactory.PrepareAddProductToOrderModelAsync(new AddProductToOrderModel(), order, product);
            model.Warnings.AddRange(warnings);

            return View(model);
        }

        #endregion

        #region Payments and other order workflow

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public virtual async Task<IActionResult> CancelOrder(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                await _orderProcessingService.CancelOrderAsync(order, true);
                await LogEditOrderAsync(order.Id);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("captureorder")]
        public virtual async Task<IActionResult> CaptureOrder(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                var errors = await _orderProcessingService.CaptureAsync(order);
                await LogEditOrderAsync(order.Id);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markorderaspaid")]
        public virtual async Task<IActionResult> MarkOrderAsPaid(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                await _orderProcessingService.MarkOrderAsPaidAsync(order);
                await LogEditOrderAsync(order.Id);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorder")]
        public virtual async Task<IActionResult> RefundOrder(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                var errors = await _orderProcessingService.RefundAsync(order);
                await LogEditOrderAsync(order.Id);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorderoffline")]
        public virtual async Task<IActionResult> RefundOrderOffline(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                await _orderProcessingService.RefundOfflineAsync(order);
                await LogEditOrderAsync(order.Id);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorder")]
        public virtual async Task<IActionResult> VoidOrder(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                var errors = await _orderProcessingService.VoidAsync(order);
                await LogEditOrderAsync(order.Id);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorderoffline")]
        public virtual async Task<IActionResult> VoidOrderOffline(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                await _orderProcessingService.VoidOfflineAsync(order);
                await LogEditOrderAsync(order.Id);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundorder")]
        public virtual async Task<IActionResult> PartiallyRefundOrderPopup(int id, bool online, OrderModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                var amountToRefund = model.AmountToRefund;
                if (amountToRefund <= decimal.Zero)
                    throw new NopException("Enter amount to refund");

                var maxAmountToRefund = order.OrderTotal - order.RefundedAmount;
                if (amountToRefund > maxAmountToRefund)
                    amountToRefund = maxAmountToRefund;

                var errors = new List<string>();
                if (online)
                    errors = (await _orderProcessingService.PartiallyRefundAsync(order, amountToRefund)).ToList();
                else
                    await _orderProcessingService.PartiallyRefundOfflineAsync(order, amountToRefund);

                await LogEditOrderAsync(order.Id);

                if (!errors.Any())
                {
                    //success
                    ViewBag.RefreshPage = true;

                    //prepare model
                    model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

                    return View(model);
                }

                //prepare model
                model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderStatus")]
        public virtual async Task<IActionResult> ChangeOrderStatus(int id, OrderModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            try
            {
                var oldOrder = order.MyDeepClone();
                var prevOrderStatus = order.OrderStatus;

                order.OrderStatusId = model.OrderStatusId;
                await _orderService.UpdateOrderAsync(order);

                await _eventPublisher.PublishAsync(new OrderStatusChangedEvent(order, prevOrderStatus));

                await _eventPublisher.PublishAsync(new AuditUpdatedEntityEvent<Order>(order, oldOrder));

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Order status has been edited. New status: {await _localizationService.GetLocalizedEnumAsync(order.OrderStatus)}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await LogEditOrderAsync(order.Id);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                model = await _orderModelFactory.PrepareOrderModelAsync(model, order);

                await _notificationService.ErrorNotificationAsync(exc);
                return View(model);
            }
        }

        #endregion

        #region delete

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            await _orderProcessingService.DeleteOrderAsync(order);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteOrder",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteOrder"), order.Id), order);

            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> PdfInvoice(int orderId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to their orders
            if (!await HasAccessToOrderAsync(orderId))
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();

            var order = await _orderService.GetOrderByIdAsync(orderId);

            byte[] bytes;
            await using var stream = new MemoryStream();

            await _pdfService.PrintOrderToPdfAsync(stream, order, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), store: null, vendor: currentVendor);
            bytes = stream.ToArray();

            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.CustomOrderNumber}.pdf");
        }

        [HttpPost, ActionName("PdfInvoice")]
        [FormValueRequired("pdf-invoice-all")]
        public virtual async Task<IActionResult> PdfInvoiceAll(OrderSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.VendorId = currentVendor.Id;
            }

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = await _productService.GetProductByIdAsync(model.ProductId);
            if (product != null && (currentVendor == null || product.VendorId == currentVendor.Id))
                filterByProductId = model.ProductId;

            //load orders
            var orders = await _orderService.SearchOrdersAsync(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingPhone: model.BillingPhone,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            //ensure that we at least one order selected
            if (!orders.Any())
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
                return RedirectToAction("List");
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/zip", "orders.zip");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> PdfInvoiceSelected(string selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(await _orderService.GetOrdersByIdsAsync(ids));
            }

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                orders = await orders.WhereAwait(HasAccessToOrderAsync).ToListAsync();
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/zip", "orders.zip");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        //currently we use this method on the add product to order details pages
        [HttpPost]
        public virtual async Task<IActionResult> ProductDetails_AttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                    if (!conditionMet.HasValue)
                        continue;

                    if (conditionMet.Value)
                        enabledAttributeMappingIds.Add(attribute.Id);
                    else
                        disabledAttributeMappingIds.Add(attribute.Id);
                }
            }

            return Json(new
            {
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveCC")]
        public virtual async Task<IActionResult> EditCreditCardInfo(int id, OrderModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            if (order.AllowStoringCreditCardNumber)
            {
                var cardType = model.CardType;
                var cardName = model.CardName;
                var cardNumber = model.CardNumber;
                var cardCvv2 = model.CardCvv2;
                var cardExpirationMonth = model.CardExpirationMonth;
                var cardExpirationYear = model.CardExpirationYear;

                order.CardType = _encryptionService.EncryptText(cardType);
                order.CardName = _encryptionService.EncryptText(cardName);
                order.CardNumber = _encryptionService.EncryptText(cardNumber);
                order.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
                order.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
                order.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
                order.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
                await _orderService.UpdateOrderAsync(order);
            }

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Credit card info has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderTotals")]
        public virtual async Task<IActionResult> EditOrderTotals(int id, OrderModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            order.OrderSubtotalInclTax = model.OrderSubtotalInclTaxValue;
            order.OrderSubtotalExclTax = model.OrderSubtotalExclTaxValue;
            order.OrderSubTotalDiscountInclTax = model.OrderSubTotalDiscountInclTaxValue;
            order.OrderSubTotalDiscountExclTax = model.OrderSubTotalDiscountExclTaxValue;
            order.OrderShippingInclTax = model.OrderShippingInclTaxValue;
            order.OrderShippingExclTax = model.OrderShippingExclTaxValue;
            order.PaymentMethodAdditionalFeeInclTax = model.PaymentMethodAdditionalFeeInclTaxValue;
            order.PaymentMethodAdditionalFeeExclTax = model.PaymentMethodAdditionalFeeExclTaxValue;
            order.TaxRates = model.TaxRatesValue;
            order.OrderTax = model.TaxValue;
            order.OrderDiscount = model.OrderTotalDiscountValue;
            order.OrderTotal = model.OrderTotalValue;
            await _orderService.UpdateOrderAsync(order);

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order totals have been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            return RedirectToAction("Edit", new { id = order.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("save-shipping-method")]
        public virtual async Task<IActionResult> EditShippingMethod(int id, OrderModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });


            var oldOrder = order.MyDeepClone();
            order.ShippingMethod = model.ShippingMethod;
            await _orderService.UpdateOrderAsync(order);

            await _eventPublisher.PublishAsync(new AuditUpdatedEntityEvent<Order>(order, oldOrder));


            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Shipping method has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            //selected card
            SaveSelectedCardName("order-billing-shipping");

            return RedirectToAction("Edit", new { id = order.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOrderItem")]
        public virtual async Task<IActionResult> EditOrderItem(int id, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnSaveOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnSaveOrderItem".Length..]);

            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            var oldOrderItem = orderItem.MyDeepClone();

            if (!decimal.TryParse(form["pvUnitPriceInclTax" + orderItemId], out var unitPriceInclTax))
                unitPriceInclTax = orderItem.UnitPriceInclTax;
            if (!decimal.TryParse(form["pvUnitPriceExclTax" + orderItemId], out var unitPriceExclTax))
                unitPriceExclTax = orderItem.UnitPriceExclTax;
            if (!int.TryParse(form["pvQuantity" + orderItemId], out var quantity))
                quantity = orderItem.Quantity;
            if (!decimal.TryParse(form["pvDiscountInclTax" + orderItemId], out var discountInclTax))
                discountInclTax = orderItem.DiscountAmountInclTax;
            if (!decimal.TryParse(form["pvDiscountExclTax" + orderItemId], out var discountExclTax))
                discountExclTax = orderItem.DiscountAmountExclTax;
            if (!decimal.TryParse(form["pvPriceInclTax" + orderItemId], out var priceInclTax))
                priceInclTax = orderItem.PriceInclTax;
            if (!decimal.TryParse(form["pvPriceExclTax" + orderItemId], out var priceExclTax))
                priceExclTax = orderItem.PriceExclTax;

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            if (quantity > 0)
            {
                var qtyDifference = orderItem.Quantity - quantity;

                if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
                {
                    orderItem.UnitPriceInclTax = unitPriceInclTax;
                    orderItem.UnitPriceExclTax = unitPriceExclTax;
                    orderItem.Quantity = quantity;
                    orderItem.DiscountAmountInclTax = discountInclTax;
                    orderItem.DiscountAmountExclTax = discountExclTax;
                    orderItem.PriceInclTax = priceInclTax;
                    orderItem.PriceExclTax = priceExclTax;
                    await _orderService.UpdateOrderItemAsync(orderItem);
                }

                //adjust inventory
                await _productService.AdjustInventoryAsync(product, qtyDifference, orderItem.AttributesXml,
                    string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));
            }
            else
            {
                //adjust inventory
                await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                await _orderService.DeleteOrderItemAsync(orderItem);
            }

            //update order totals
            var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
            {
                PriceInclTax = unitPriceInclTax,
                PriceExclTax = unitPriceExclTax,
                DiscountAmountInclTax = discountInclTax,
                DiscountAmountExclTax = discountExclTax,
                SubTotalInclTax = priceInclTax,
                SubTotalExclTax = priceExclTax,
                Quantity = quantity
            };
            await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

            await _eventPublisher.PublishAsync(new AuditUpdatedEntityEvent<OrderItem>(orderItem, oldOrderItem, order.Id.ToString()));

            //add a note
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order item has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            await LogEditOrderAsync(order.Id);

            foreach (var warning in updateOrderParameters.Warnings)
                _notificationService.WarningNotification(warning);

            //selected card
            SaveSelectedCardName("order-products");

            return RedirectToAction("Edit", new { id = order.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteOrderItem")]
        public virtual async Task<IActionResult> DeleteOrderItem(int id, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnDeleteOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnDeleteOrderItem".Length..]);

            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            if ((await _giftCardService.GetGiftCardsByPurchasedWithOrderItemIdAsync(orderItem.Id)).Any())
            {
                //we cannot delete an order item with associated gift cards
                //a store owner should delete them first

                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.OrderItem.DeleteAssociatedGiftCardRecordError"));
            }
            else
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                //adjust inventory
                await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                await _orderService.DeleteOrderItemAsync(orderItem);

                await _eventPublisher.PublishAsync(new AuditDeletedEntityEvent<OrderItem>(orderItem, order.Id.ToString()));

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters(order, orderItem);
                await _orderProcessingService.UpdateOrderTotalsAsync(updateOrderParameters);

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "Order item has been deleted",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await LogEditOrderAsync(order.Id);

                foreach (var warning in updateOrderParameters.Warnings)
                    _notificationService.WarningNotification(warning);
            }

            //selected card
            SaveSelectedCardName("order-products");

            return RedirectToAction("Edit", new { id = order.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
        public virtual async Task<IActionResult> ResetDownloadCount(int id, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnResetDownloadCount".Length..]);

            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            orderItem.DownloadCount = 0;
            await _orderService.UpdateOrderItemAsync(orderItem);
            await LogEditOrderAsync(order.Id);

            //selected card
            SaveSelectedCardName("order-products");

            return RedirectToAction("Edit", new { id = order.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
        public virtual async Task<IActionResult> ActivateDownloadItem(int id, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnPvActivateDownload".Length..]);

            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            orderItem.IsDownloadActivated = !orderItem.IsDownloadActivated;
            await _orderService.UpdateOrderItemAsync(orderItem);

            await LogEditOrderAsync(order.Id);

            //selected card
            SaveSelectedCardName("order-products");

            return RedirectToAction("Edit", new { id = order.Id });
        }

        public virtual async Task<IActionResult> UploadLicenseFilePopup(int id, int orderItemId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //try to get an order item with the specified id
            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId)
                ?? throw new ArgumentException("No product found with the specified order item id");

            if (!product.IsDownload)
                throw new ArgumentException("Product is not downloadable");

            //ensure a vendor has access only to his products 
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToProductAsync(orderItem))
                return RedirectToAction("List");

            //prepare model
            var model = await _orderModelFactory.PrepareUploadLicenseModelAsync(new UploadLicenseModel(), order, orderItem);

            return View(model);
        }

        #endregion

        #region Addresses

        public virtual async Task<IActionResult> AddressEdit(int addressId, int orderId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an address with the specified id
            var address = await _addressService.GetAddressByIdAsync(addressId)
                ?? throw new ArgumentException("No address found with the specified id", nameof(addressId));

            //prepare model
            var model = await _orderModelFactory.PrepareOrderAddressModelAsync(new OrderAddressModel(), order, address);

            return View("~/Plugins/ZarIran.Misc.ExtendedOrder/Areas/Admin/Views/ExtendedOrder/AddressEdit.cshtml", model);


        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressEdit(OrderAddressModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", new { id = order.Id });

            //try to get an address with the specified id
            var address = await _addressService.GetAddressByIdAsync(model.Address.Id)
                ?? throw new ArgumentException("No address found with the specified id");

            var oldAdress = address.MyDeepClone();
            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                await _addressService.UpdateAddressAsync(address);

                await _eventPublisher.PublishAsync(new AuditUpdatedEntityEvent<Address>(address, oldAdress, order.Id.ToString()));
                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "Address has been edited",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await LogEditOrderAsync(order.Id);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Updated"));

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, orderId = model.OrderId });
            }

            //prepare model
            model = await _orderModelFactory.PrepareOrderAddressModelAsync(model, order, address);

            //if we got this far, something failed, redisplay form
            return View("~/Plugins/ZarIran.Misc.ExtendedOrder/Areas/Admin/Views/ExtendedOrder/AddressEdit.cshtml", model);
        }
        #endregion

        #region OrderNote

        [HttpPost]
        public virtual async Task<IActionResult> OrderNoteDelete(int id, int orderId)
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsInCustomerRoleAsync(currentCustomer, "Administrators"))
                return AccessDeniedView();

            //try to get an order with the specified id
            _ = await _orderService.GetOrderByIdAsync(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an order note with the specified id
            var orderNote = await _orderService.GetOrderNoteByIdAsync(id)
                ?? throw new ArgumentException("No order note found with the specified id");

            await _orderService.DeleteOrderNoteAsync(orderNote);

            return new NullJsonResult();
        }

        #endregion

        #endregion

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

        #endregion
    }
}
