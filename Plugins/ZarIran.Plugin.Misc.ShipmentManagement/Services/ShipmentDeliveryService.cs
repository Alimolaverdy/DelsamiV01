using ZarIran.Plugin.Misc.Core.Helpers;
using ZarIran.Plugin.Misc.ShipmentManagement.Domain;
using ZarIran.Plugin.Misc.ShipmentManagement.Helpers;
using ZarIran.Plugin.Misc.ShipmentManagement.Models;
using ZarIran.Plugin.Misc.ShipmentManagement.Models.Apis;
using ZarIran.Plugin.Payment.Core.Domain;
using ZarIran.Plugin.Payment.Core.Services;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Services
{
    public class ShipmentDeliveryService : IShipmentDeliveryService
    {
        #region Fields

        private readonly IRepository<ShipmentDelivery> _shipmentDeliveryRepo;
        private readonly ShipmentManagementSettings _shipmentManagementSettings;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private readonly IAddressService _addressService;
        private readonly IProductService _productService;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        #endregion

        #region Ctor

        public ShipmentDeliveryService(IRepository<ShipmentDelivery> shipmentDeliveryRepo,
                                       ShipmentManagementSettings shipmentManagementSettings,
                                       IOrderService orderService,
                                       IShipmentService shipmentService,
                                       IEventPublisher eventPublisher,
                                       IPaymentService paymentService,
                                       ILogger logger,
                                       IAddressService addressService,
                                       IProductService productService)
        {
            _shipmentDeliveryRepo = shipmentDeliveryRepo;
            _shipmentManagementSettings = shipmentManagementSettings;
            _orderService = orderService;
            _shipmentService = shipmentService;
            _eventPublisher = eventPublisher;
            _paymentService = paymentService;
            _logger = logger;
            _addressService = addressService;
            _productService = productService;
        }

        #endregion

        #region Methods

        public async Task<ShipmentDelivery> GetShipmentDeliveryByShipmentIdAsync(int shipmentId)
        {
            return await _shipmentDeliveryRepo.Table.FirstOrDefaultAsync(w => w.ShipmentId == shipmentId);
        }

        public async Task InsertAsync(ShipmentDelivery shipmentAdditionalData)
        {
            await _shipmentDeliveryRepo.InsertAsync(shipmentAdditionalData);
        }

        public async Task<bool> CheckDeliveryCodeIsDuplicated(int deliveryCode)
        {
            return await _shipmentDeliveryRepo.Table.AnyAsync(w => w.DeliveryCode == deliveryCode);
        }

        public async Task<(bool Result, string ErrorMessage)> ValidateDeliveryCodeAsync(string shipmentCode, int deliveryCode)
        {
            await _logger.InformationAsync($"shipmentCode:{shipmentCode} AND deliveryCode:{deliveryCode} ");

            var semaphor = _locks.GetOrAdd(shipmentCode, _ => new SemaphoreSlim(1, 1));

            await semaphor.WaitAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(shipmentCode))
                    return (false, "کد مرسوله خالی است");

                if (deliveryCode <= 0)
                    return (false, "کد تحویل درست وارد نشده است");
                try
                {
                    var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(shipmentCode);

                    var shipmentDelivery = await _shipmentDeliveryRepo.Table.FirstOrDefaultAsync(w => w.ShipmentId == shipmentId);

                    if (shipmentDelivery == null)
                        return (false, "مرسوله یافت نشد");

                    if (shipmentDelivery.DeliveryCode != deliveryCode)
                        return (false, "کد تحویل اشتباه است");

                    if (shipmentDelivery.Delivered)
                        return (false, "این مرسوله قبلا تحویل شده است");

                    shipmentDelivery.Delivered = true;
                    shipmentDelivery.DeliveredOnUtc = DateTime.UtcNow;

                    await _shipmentDeliveryRepo.UpdateAsync(shipmentDelivery);

                    var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentDelivery.ShipmentId);

                    if (shipment != null)
                    {
                        shipment.DeliveryDateUtc = DateTime.UtcNow;

                        await _shipmentService.UpdateShipmentAsync(shipment);

                        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

                        if (order != null)
                        {
                            order.OrderStatusId = (int)OrderStatus.Complete;
                            order.ShippingStatusId = (int)ShippingStatus.Delivered;
                            order.PaymentStatusId = (int)PaymentStatus.Paid;

                            await _orderService.UpdateOrderAsync(order);
                            //await _eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));

                            var orderStatusChangedEvent = new OrderStatusChangedEvent(order, OrderStatus.Processing);

                            await _eventPublisher.PublishAsync(orderStatusChangedEvent);
                        }
                    }

                    return (true, "");
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync(ex.Message, ex);

                    return (false, "خطای ناشناخته");
                }
            }
            finally
            {
                semaphor.Release();
                _locks.TryRemove(shipmentCode, out _); // Optional: برای پاک کردن قفل بعد از انجام کار
            }
        }

        public async Task<(bool Result, string ErrorMessage)> ValidateCODDeliveryCodeAsync(bool hasDeliveryCode, int? deliveryCode, string shipmentCode)
        {
            await _logger.InformationAsync($"COD shipmentCode:{shipmentCode} AND deliveryCode:{deliveryCode} ");

            if (string.IsNullOrWhiteSpace(shipmentCode))
                return (false, "کد مرسوله خالی است");


            var semaphor = _locks.GetOrAdd(shipmentCode, _ => new SemaphoreSlim(1, 1));

            await semaphor.WaitAsync();
            try
            {
                try
                {
                    var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(shipmentCode);

                    var shipmentDelivery = await _shipmentDeliveryRepo.Table.FirstOrDefaultAsync(w => w.ShipmentId == shipmentId);

                    if (shipmentDelivery == null)
                        return (false, "مرسوله یافت نشد");

                    if (shipmentDelivery.Delivered)
                        return (false, "این مرسوله قبلا تحویل شده است");

                    var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentDelivery.ShipmentId);

                    var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

                    if (!order.PaymentMethodSystemName.Equals("Payments.CheckMoneyOrder", StringComparison.InvariantCultureIgnoreCase))
                        return (false, "روش پرداخت سفارش، پرداخت در محل نمی باشد");

                    if (hasDeliveryCode)
                    {
                        if (deliveryCode <= 0)
                            return (false, "کد تحویل درست وارد نشده است");

                        if (shipmentDelivery.DeliveryCode != deliveryCode)
                            return (false, "کد تحویل اشتباه است");
                    }


                    shipmentDelivery.Delivered = true;
                    shipmentDelivery.DeliveredOnUtc = DateTime.UtcNow;

                    await _shipmentDeliveryRepo.UpdateAsync(shipmentDelivery);


                    if (shipment != null)
                    {
                        shipment.DeliveryDateUtc = DateTime.UtcNow;

                        await _shipmentService.UpdateShipmentAsync(shipment);


                        if (order != null)
                        {
                            order.OrderStatusId = (int)OrderStatus.Complete;
                            order.ShippingStatusId = (int)ShippingStatus.Delivered;
                            order.PaymentStatusId = (int)PaymentStatus.Paid;

                            await _orderService.UpdateOrderAsync(order);
                            //await _eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));

                            var orderStatusChangedEvent = new OrderStatusChangedEvent(order, OrderStatus.Processing);

                            await _eventPublisher.PublishAsync(orderStatusChangedEvent);
                        }
                    }

                    return (true, "");
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync(ex.Message, ex);

                    return (false, "خطای ناشناخته");
                }
            }
            finally
            {
                semaphor.Release();
                _locks.TryRemove(shipmentCode, out _);
            }
        }


        public async Task<(bool Result, string ErrorMessage)> CheckDeliveryCodeAsync(string shipmentCode, int deliveryCode)
        {
            await _logger.InformationAsync($"shipmentCode:{shipmentCode} AND deliveryCode:{deliveryCode} ");

            if (string.IsNullOrWhiteSpace(shipmentCode))
                return (false, "کد مرسوله خالی است");

            if (deliveryCode <= 0)
                return (false, "کد تحویل درست وارد نشده است");
            try
            {
                var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(shipmentCode);

                var shipmentDelivery = await _shipmentDeliveryRepo.Table.FirstOrDefaultAsync(w => w.ShipmentId == shipmentId);

                if (shipmentDelivery == null)
                    return (false, "مرسوله یافت نشد");

                if (shipmentDelivery.DeliveryCode != deliveryCode)
                    return (false, "کد تحویل اشتباه است");

                return (true, "");
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);

                return (false, "خطای ناشناخته");
            }
        }

        public async Task<(bool Result, string ErrorMessage)> ShippedAsync(string shipmentCode)
        {
            await _logger.InformationAsync($"shipmentCode:{shipmentCode}");

            shipmentCode = NormalizeHelper.NormalizeNumbers(shipmentCode);

            if (string.IsNullOrWhiteSpace(shipmentCode))
                return (false, "کد مرسوله خالی است");

            try
            {
                var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(shipmentCode);

                var shipmentDelivery = await _shipmentDeliveryRepo.Table.FirstOrDefaultAsync(w => w.ShipmentId == shipmentId);

                if (shipmentDelivery == null)
                    return (false, "مرسوله یافت نشد");

                if (shipmentDelivery.Delivered)
                    return (false, "این مرسوله قبلا تحویل شده است");

                if (shipmentDelivery.Shipped)
                    return (false, "این مرسوله قبلا ارسال شده است");

                shipmentDelivery.Shipped = true;
                shipmentDelivery.ShippedOnUtc = DateTime.UtcNow;

                await _shipmentDeliveryRepo.UpdateAsync(shipmentDelivery);

                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentDelivery.ShipmentId);

                if (shipment != null)
                {
                    var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

                    if (order != null)
                    {
                        order.ShippingStatusId = (int)ShippingStatus.Shipped;

                        await _orderService.UpdateOrderAsync(order);
                    }
                }

                return (true, "");
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);

                return (false, "خطای ناشناخته");
            }
        }

        public async Task<(bool Result, string ErrorMessage)> InsertPaymentAsync(InsertPaymentRequestModel paymentRequest)
        {
            await _logger.InformationAsync(Newtonsoft.Json.JsonConvert.SerializeObject(paymentRequest));

            if (string.IsNullOrWhiteSpace(paymentRequest.ShipmentCode))
                return (false, "کد مرسوله خالی است");

            try
            {
                var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(paymentRequest.ShipmentCode);

                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);

                if (shipment == null)
                    return (false, "مرسوله یافت نشد");

                var order = await _orderService.GetOrderByIdAsync((int)shipment.OrderId);

                if (order == null)
                    return (false, "سفارش یافت نشد");

                var payment = new Payment.Core.Domain.Payment
                {
                    Amount = paymentRequest.Amount,
                    Completed = true,
                    CreatedDateTime = DateTime.UtcNow,
                    VerifyDateTime = DateTime.UtcNow,
                    CustomerId = order.CustomerId,
                    Gateway = paymentRequest.Gateway,
                    OrderId = order.Id,
                    PaymentType = Payment.Core.Enums.PaymentType.COD,
                    ReferenceNumber = paymentRequest.ReferenceNumber,
                    SourceType = Payment.Core.Enums.SourceType.Order,
                    Status = Payment.Core.Enums.PaymentStatus.Verified,
                    TrackId = paymentRequest.TrackId,
                    TeminalNo = paymentRequest.PoseCode
                };

                await _paymentService.InsertAsync(payment);

                return (true, "");
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);

                return (false, "");
            }
        }


        public async Task<(ZarIranOrderDetailModel Result, string ErrorMessage)> GetOrderDetailsByShipmentIdAsync(string shipmentCode)
        {
            var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(shipmentCode);
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return (null, "مرسوله‌ای با این کد پیدا نشد.");

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

            if (order == null)
                return (null, "سفارشی برای این مرسوله یافت نشد.");

            if (!order.PaymentMethodSystemName.Equals("Payments.CheckMoneyOrder", StringComparison.InvariantCultureIgnoreCase))
                return (null, "روش پرداخت سفارش، پرداخت در محل نمی باشد");

            if (order.OrderStatus == OrderStatus.Complete)
                return (null, "سفارش قبلا پرداخت شده است");

            if (order.OrderStatus != OrderStatus.Processing)
                return (null, "سفارش در وضعیت معتبر قرار ندارد");

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            var productIds = orderItems.Select(i => i.ProductId).Distinct().ToArray();
            var products = await _productService.GetProductsByIdsAsync(productIds);
            var productNamesDict = products.ToDictionary(p => p.Id, p => p.Name);

            Address address = null;
            if (order.ShippingAddressId.HasValue)
                address = await _addressService?.GetAddressByIdAsync(order.ShippingAddressId.Value);

            var customerInfo = MapToCustomerInfo(address);
            var orderItemModels = orderItems
                .Select(item => MapToOrderItemModel(item, productNamesDict))
                .ToList();

            var resultModel = new ZarIranOrderDetailModel
            {
                OrderSubtotalExclTax = order.OrderSubtotalExclTax,
                OrderTotal = order.OrderTotal,
                CustomerInfo = customerInfo,
                OrderItems = orderItemModels
            };

            return (resultModel, null);
        }

        public async Task<(bool Result, string ErrorMessage, List<Payment.Core.Domain.Payment> Payments)> InsertCODPaymentsAsync(InsertCODPaymentsRequestModel paymentsRequest)
        {
            List<Payment.Core.Domain.Payment> payments = new();

            await _logger.InformationAsync(Newtonsoft.Json.JsonConvert.SerializeObject(paymentsRequest));

            if (string.IsNullOrWhiteSpace(paymentsRequest.ShipmentCode))
                return (false, "کد مرسوله خالی است", payments);

            try
            {
                var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(paymentsRequest.ShipmentCode);

                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);

                if (shipment == null)
                    return (false, "مرسوله یافت نشد", payments);

                var order = await _orderService.GetOrderByIdAsync((int)shipment.OrderId);

                if (order == null)
                    return (false, "سفارش یافت نشد", payments);


                foreach (var paymentRequest in paymentsRequest.Payments)
                {

                    var payment = new Payment.Core.Domain.Payment
                    {
                        Amount = paymentRequest.Amount,
                        Completed = true,
                        CreatedDateTime = paymentRequest.PayDate.ToUniversalTime(),
                        VerifyDateTime = DateTime.UtcNow,
                        CustomerId = order.CustomerId,
                        Gateway = paymentRequest.Gateway,
                        OrderId = order.Id,
                        PaymentType = Payment.Core.Enums.PaymentType.COD,
                        ReferenceNumber = paymentRequest.ReferenceNumber,
                        SourceType = Payment.Core.Enums.SourceType.Order,
                        Status = Payment.Core.Enums.PaymentStatus.Verified,
                        TrackId = paymentRequest.TrackingId,
                        TeminalNo = paymentRequest.TerminalNumber
                    };

                    await _paymentService.InsertAsync(payment);

                    payments.Add(payment);
                }

                return (true, "", payments);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);

                return (false, "", payments);
            }
        }

        public async Task<(bool Result, string ErrorMessage, Payment.Core.Domain.Payment Payment)> InsertCODPaymentAsync(InsertCODPaymentRequestModel paymentRequest)
        {
            await _logger.InformationAsync(Newtonsoft.Json.JsonConvert.SerializeObject(paymentRequest));

            if (string.IsNullOrWhiteSpace(paymentRequest.ShipmentCode))
                return (false, "کد مرسوله خالی است", null);

            try
            {
                var shipmentId = ShipmentManagementHelper.GetShipmentIdFromShipmentCode(paymentRequest.ShipmentCode);

                var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);

                if (shipment == null)
                    return (false, "مرسوله یافت نشد", null);

                var order = await _orderService.GetOrderByIdAsync((int)shipment.OrderId);

                if (order == null)
                    return (false, "سفارش یافت نشد", null);

                var payment = new Payment.Core.Domain.Payment
                {
                    Amount = paymentRequest.Amount,
                    Completed = true,
                    CreatedDateTime = paymentRequest.PayDate.ToUniversalTime(),
                    VerifyDateTime = DateTime.UtcNow,
                    CustomerId = order.CustomerId,
                    Gateway = paymentRequest.Gateway,
                    OrderId = order.Id,
                    PaymentType = Payment.Core.Enums.PaymentType.COD,
                    ReferenceNumber = paymentRequest.ReferenceNumber,
                    SourceType = Payment.Core.Enums.SourceType.Order,
                    Status = Payment.Core.Enums.PaymentStatus.Verified,
                    TrackId = paymentRequest.TrackingId,
                    TeminalNo = paymentRequest.TerminalNumber
                };

                await _paymentService.InsertAsync(payment);

                return (true, "", payment);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);

                return (false, "", null);
            }
        }
        #endregion

        #region Utilities


        private ZarIranCustomerInfoModel MapToCustomerInfo(Address address)
        {
            if (address == null)
                return null;

            return new ZarIranCustomerInfoModel
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                ShippingAddress = address.Address1
            };
        }

        private ZarIranOrderItemModel MapToOrderItemModel(OrderItem item, Dictionary<int, string> productNamesDict)
        {
            productNamesDict.TryGetValue(item.ProductId, out var productName);

            return new ZarIranOrderItemModel
            {
                ProductName = productName,
                Quantity = item.Quantity,
                UnitPriceInclTax = item.UnitPriceInclTax
            };
        }

        #endregion
    }
}
