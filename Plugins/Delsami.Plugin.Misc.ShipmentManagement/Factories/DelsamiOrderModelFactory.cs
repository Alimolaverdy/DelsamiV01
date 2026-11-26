using Delsami.Plugin.Misc.ShipmentManagement.Models;
using Delsami.Plugin.Misc.ShipmentManagement.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement.Factories;

public class DelsamiOrderModelFactory : IDelsamiOrderModelFactory
{
    #region Fields

    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IOrderShippingMethodCapacityMappingService _OrderShippingMethodService;

    private readonly ILocalizationService _localizationService;
    private readonly OrderSettings _orderSettings;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly PdfSettings _pdfSettings;
    private readonly IShippingService _shippingService;
    private readonly IAddressModelFactory _addressModelFactory;
    private readonly IAddressService _addressService;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly ICountryService _countryService;
    private readonly AddressSettings _addressSettings;
    private readonly IShipmentService _shipmentService;
    private readonly IWorkContext _workContext;
    private readonly ICustomerService _customerService;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IPaymentService _paymentService;
    private readonly TaxSettings _taxSettings;
    private readonly ICurrencyService _currencyService;
    private readonly IPriceFormatter _priceFormatter;
    private readonly IOrderService _orderService;
    private readonly CatalogSettings _catalogSettings;
    private readonly IGiftCardService _giftCardService;
    private readonly IRewardPointService _rewardPointService;
    private readonly VendorSettings _vendorSettings;
    private readonly MediaSettings _mediaSettings;
    private readonly IProductService _productService;
    private readonly IVendorService _vendorService;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IStoreContext _storeContext;
    private readonly IWebHelper _webHelper;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IPictureService _pictureService;
    private readonly IShipmentDeliveryService _shipmentDeliveryService;
    private readonly ShipmentManagementSettings _shipmentSettings;
    private string _tipaxSystemName = "Delsami.Shipping.Tipax";
    private string _postSystemName = "Delsami.Shipping.IRPost";

    #endregion

    #region Ctor

    public DelsamiOrderModelFactory(IDateTimeHelper dateTimeHelper,
                                     ILocalizationService localizationService,
                                     OrderSettings orderSettings,
                                     IOrderShippingMethodCapacityMappingService OrderShippingMethodService,
                                     ShipmentManagementSettings shipmentSettings,
                                     IOrderProcessingService orderProcessingService,
                                     PdfSettings pdfSettings,
                                     IAddressModelFactory addressModelFactory,
                                     IAddressService addressService,
                                     IStateProvinceService stateProvinceService,
                                     ICountryService countryService,
                                     IShippingService shippingService,
                                     AddressSettings addressSettings,
                                     IShipmentService shipmentService,
                                     IWorkContext workContext,
                                     ICustomerService customerService,
                                     IPaymentPluginManager paymentPluginManager,
                                     IPaymentService paymentService,
                                     TaxSettings taxSettings,
                                     ICurrencyService currencyService,
                                     IPriceFormatter priceFormatter,
                                     IOrderService orderService,
                                     CatalogSettings catalogSettings,
                                     IGiftCardService giftCardService,
                                     IRewardPointService rewardPointService,
                                     VendorSettings vendorSettings,
                                     MediaSettings mediaSettings,
                                     IProductService productService,
                                     IVendorService vendorService,
                                     IUrlRecordService urlRecordService,
                                     IStoreContext storeContext,
                                     IWebHelper webHelper,
                                     IStaticCacheManager staticCacheManager,
                                     IPictureService pictureService,
                                     IShipmentDeliveryService shipmentDeliveryService)
    {
        _dateTimeHelper = dateTimeHelper;
        _OrderShippingMethodService = OrderShippingMethodService;
        _shipmentSettings = shipmentSettings;
        _localizationService = localizationService;
        _orderSettings = orderSettings;
        _shippingService = shippingService;
        _orderProcessingService = orderProcessingService;
        _pdfSettings = pdfSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _stateProvinceService = stateProvinceService;
        _countryService = countryService;
        _addressSettings = addressSettings;
        _shipmentService = shipmentService;
        _workContext = workContext;
        _customerService = customerService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _taxSettings = taxSettings;
        _currencyService = currencyService;
        _priceFormatter = priceFormatter;
        _orderService = orderService;
        _catalogSettings = catalogSettings;
        _giftCardService = giftCardService;
        _rewardPointService = rewardPointService;
        _vendorSettings = vendorSettings;
        _mediaSettings = mediaSettings;
        _productService = productService;
        _vendorService = vendorService;
        _urlRecordService = urlRecordService;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _staticCacheManager = staticCacheManager;
        _pictureService = pictureService;
        _shipmentDeliveryService = shipmentDeliveryService;
    }

    #endregion

    #region Methods

    public virtual async Task<DelsamiOrderDetailsModel> PrepareOrderDetailsModelAsync(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));
        var model = new DelsamiOrderDetailsModel
        {
            Id = order.Id,
            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
            OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
            OrderStatusEnum = order.OrderStatus,
            IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
            IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
            PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending,
            CustomOrderNumber = order.CustomOrderNumber,

            //shipping info
            ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus)
        };
        if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
        {
            model.IsShippable = true;
            model.PickupInStore = order.PickupInStore;
            if (!order.PickupInStore)
            {
                var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);

                await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress,
                    address: shippingAddress,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
            }
            else if (order.PickupAddressId.HasValue && await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
            {
                model.PickupAddress = new AddressModel
                {
                    Address1 = pickupAddress.Address1,
                    City = pickupAddress.City,
                    County = pickupAddress.County,
                    StateProvinceName = await _stateProvinceService.GetStateProvinceByAddressAsync(pickupAddress) is StateProvince stateProvince
                        ? await _localizationService.GetLocalizedAsync(stateProvince, entity => entity.Name)
                        : string.Empty,
                    CountryName = await _countryService.GetCountryByAddressAsync(pickupAddress) is Country country
                        ? await _localizationService.GetLocalizedAsync(country, entity => entity.Name)
                        : string.Empty,
                    ZipPostalCode = pickupAddress.ZipPostalCode
                };
            }


            model.ShippingMethod = order.ShippingMethod;

            model.ShippingMethodId = (await _shippingService.GetAllShippingMethodsAsync()).FirstOrDefault(a => a.Name == order.ShippingMethod)?.Id ?? 0;

            var deliverydate = await _OrderShippingMethodService.GetOrderShippingMethodCapacityMappingByOrderIdAsync(order.Id);
            if (deliverydate != null)
            {

                var delivertDateTimeStr = $"{deliverydate.DeliveryDateOnUtc.ToShortDateString().ToString()} - {deliverydate.Time.Replace("-", " الی ")}";
                model.ShippingDateTime = delivertDateTimeStr;
            }

            //shipments (only already shipped or ready for pickup)
            var shipments = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).OrderBy(x => x.CreatedOnUtc).ToList();
            foreach (var shipment in shipments)
            {
                var shipmentDelivery = await _shipmentDeliveryService.GetShipmentDeliveryByShipmentIdAsync(shipment.Id);
                string trackingLink = null;
                if (order.ShippingRateComputationMethodSystemName.Equals(_postSystemName, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                    trackingLink = string.Format(_shipmentSettings.IRPostTrackingLink, shipment.TrackingNumber);

                if (order.ShippingRateComputationMethodSystemName.Equals(_tipaxSystemName, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                    trackingLink = string.Format(_shipmentSettings.TipaxTrackingLink, shipment.TrackingNumber);

                var shipmentModel = new DelsamiOrderDetailsModel.DelsamiShipmentBriefModel
                {
                    Id = shipment.Id,
                    TrackingLink = trackingLink,
                    TrackingNumber = shipment.TrackingNumber,
                    DeliveryCode = order.ShippingRateComputationMethodSystemName.Equals("Delsami.Shipping.DelsamiLogistic", StringComparison.InvariantCultureIgnoreCase) ? shipmentDelivery.DeliveryCode : 0,
                    Delivered = shipmentDelivery.Delivered,
                    ShippedDate = shipmentDelivery.ShippedOnUtc,
                    DeliveryDate = shipmentDelivery.DeliveredOnUtc,
                    CanHaveTrackingNumber = order.ShippingRateComputationMethodSystemName.Equals(_postSystemName, StringComparison.InvariantCultureIgnoreCase)
                    || order.ShippingRateComputationMethodSystemName.Equals(_tipaxSystemName, StringComparison.InvariantCultureIgnoreCase)
                };

                if (order.OrderStatus == OrderStatus.Processing)
                {
                    if (order.ShippingStatus == ShippingStatus.Shipped)
                    {
                        model.OrderStatus = string.Format(
                            await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.OrderStatus"),
                            order.ShippingMethod);
                    }
                    else if (order.ShippingStatus == ShippingStatus.NotYetShipped)
                    {
                        model.OrderStatus = await _localizationService.GetResourceAsync(
                            "Delsami.Plugin.Misc.ShipmentManagement.Processing");
                    }
                }
                else if (order.OrderStatus == OrderStatus.Complete && order.ShippingStatus == ShippingStatus.Delivered)
                {
                    model.OrderStatus = await _localizationService.GetResourceAsync(
                        "Delsami.Plugin.Misc.ShipmentManagement.Delivered");
                }
                else
                {
                    model.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                }


                if (shipment.ShippedDateUtc.HasValue)
                    shipmentModel.ShippedDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                if (shipment.ReadyForPickupDateUtc.HasValue)
                    shipmentModel.ReadyForPickupDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ReadyForPickupDateUtc.Value, DateTimeKind.Utc);
                if (shipment.DeliveryDateUtc.HasValue)
                    shipmentModel.DeliveryDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                model.Shipments.Add(shipmentModel);
            }
        }

        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //billing info
        await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress,
            address: billingAddress,
            excludeProperties: false,
            addressSettings: _addressSettings);

        //VAT number
        model.VatNumber = order.VatNumber;

        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

        //payment method
        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        var paymentMethod = await _paymentPluginManager
            .LoadPluginBySystemNameAsync(order.PaymentMethodSystemName, customer, order.StoreId);
        model.PaymentMethod = paymentMethod != null ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, languageId) : order.PaymentMethodSystemName;
        model.PaymentMethodStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
        model.CanRePostProcessPayment = await _paymentService.CanRePostProcessPaymentAsync(order);
        //custom values
        model.CustomValues = _paymentService.DeserializeCustomValues(order);

        //order subtotal
        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
        {
            //including tax

            //order subtotal
            var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
            model.OrderSubtotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            model.OrderSubtotalValue = orderSubtotalInclTaxInCustomerCurrency;
            //discount (applied to order subtotal)
            var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
            if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
            {
                model.OrderSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                model.OrderSubTotalDiscountValue = orderSubTotalDiscountInclTaxInCustomerCurrency;
            }
        }
        else
        {
            //excluding tax

            //order subtotal
            var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
            model.OrderSubtotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            model.OrderSubtotalValue = orderSubtotalExclTaxInCustomerCurrency;
            //discount (applied to order subtotal)
            var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
            if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
            {
                model.OrderSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                model.OrderSubTotalDiscountValue = orderSubTotalDiscountExclTaxInCustomerCurrency;
            }
        }

        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            //including tax

            //order shipping
            var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
            model.OrderShipping = await _priceFormatter.FormatShippingPriceAsync(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            model.OrderShippingValue = orderShippingInclTaxInCustomerCurrency;
            //payment method additional fee
            var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
            if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
            {
                model.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                model.PaymentMethodAdditionalFeeValue = paymentMethodAdditionalFeeInclTaxInCustomerCurrency;
            }
        }
        else
        {
            //excluding tax

            //order shipping
            var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
            model.OrderShipping = await _priceFormatter.FormatShippingPriceAsync(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            model.OrderShippingValue = orderShippingExclTaxInCustomerCurrency;
            //payment method additional fee
            var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
            if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
            {
                model.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                model.PaymentMethodAdditionalFeeValue = paymentMethodAdditionalFeeExclTaxInCustomerCurrency;
            }
        }

        //tax
        var displayTax = true;
        var displayTaxRates = true;
        if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            displayTax = false;
            displayTaxRates = false;
        }
        else
        {
            if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
                displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                displayTax = !displayTaxRates;

                var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                model.Tax = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

                foreach (var tr in taxRates)
                {
                    model.TaxRates.Add(new DelsamiOrderDetailsModel.TaxRate
                    {
                        Rate = _priceFormatter.FormatTaxRate(tr.Key),
                        Value = await _priceFormatter.FormatPriceAsync(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, languageId),
                    });
                }
            }
        }
        model.DisplayTaxRates = displayTaxRates;
        model.DisplayTax = displayTax;
        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
        model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

        //discount (applied to order total)
        var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
        if (orderDiscountInCustomerCurrency > decimal.Zero)
        {
            model.OrderTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
            model.OrderTotalDiscountValue = orderDiscountInCustomerCurrency;
        }

        //gift cards
        foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
        {
            model.GiftCards.Add(new DelsamiOrderDetailsModel.GiftCard
            {
                CouponCode = (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId)).GiftCardCouponCode,
                Amount = await _priceFormatter.FormatPriceAsync(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId),
            });
        }

        //reward points           
        if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
        {
            model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
            model.RedeemedRewardPointsAmount = await _priceFormatter.FormatPriceAsync(-(_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId);
        }

        //total
        var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
        model.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
        model.OrderTotalValue = orderTotalInCustomerCurrency;

        //checkout attributes
        model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

        //order notes
        foreach (var orderNote in (await _orderService.GetOrderNotesByOrderIdAsync(order.Id, true))
            .OrderByDescending(on => on.CreatedOnUtc)
            .ToList())
        {
            model.OrderNotes.Add(new DelsamiOrderDetailsModel.OrderNote
            {
                Id = orderNote.Id,
                HasDownload = orderNote.DownloadId > 0,
                Note = _orderService.FormatOrderNoteText(orderNote),
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc)
            });
        }

        //purchased products
        model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
        model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
        model.ShowProductThumbnail = _orderSettings.ShowProductThumbnailInOrderDetailsPage;

        var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

        foreach (var orderItem in orderItems)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            var orderItemModel = new DelsamiOrderDetailsModel.OrderItemModel
            {
                Id = orderItem.Id,
                OrderItemGuid = orderItem.OrderItemGuid,
                Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                VendorName = (await _vendorService.GetVendorByIdAsync(product.VendorId))?.Name ?? string.Empty,
                ProductId = product.Id,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),

                ProductSeName = await PrepareSeName(product),
                Quantity = orderItem.Quantity,
                AttributeInfo = orderItem.AttributeDescription,
            };
            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                    rentalStartDate, rentalEndDate);
            }
            model.Items.Add(orderItemModel);

            //unit price, subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                orderItemModel.UnitPriceValue = unitPriceInclTaxInCustomerCurrency;

                var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                orderItemModel.SubTotalValue = priceInclTaxInCustomerCurrency;
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                orderItemModel.UnitPriceValue = unitPriceExclTaxInCustomerCurrency;

                var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                orderItemModel.SubTotalValue = priceExclTaxInCustomerCurrency;
            }

            //downloadable products
            if (await _orderService.IsDownloadAllowedAsync(orderItem))
                orderItemModel.DownloadId = product.DownloadId;
            if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;

            if (_orderSettings.ShowProductThumbnailInOrderDetailsPage)
            {
                orderItemModel.Picture = await PrepareOrderItemPictureModelAsync(orderItem, _mediaSettings.OrderThumbPictureSize, true, orderItemModel.ProductName);
            }
        }

        return model;
    }

    private async Task<string> PrepareSeName(Product product)
    {
        var parent = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
        return await _urlRecordService.GetSeNameAsync(parent);
    }

    public virtual async Task<DelsamiCustomerOrderListModel> PrepareCustomerOrderListModelAsync()
    {
        var model = new DelsamiCustomerOrderListModel();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var orders = await _orderService.SearchOrdersAsync(storeId: store.Id,
            customerId: customer.Id);
        foreach (var order in orders)
        {
            var orderModel = new DelsamiCustomerOrderListModel.OrderDetailsModel
            {
                Id = order.Id,
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatusEnum = order.OrderStatus,
                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                CustomOrderNumber = order.CustomOrderNumber
            };
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, (await _workContext.GetWorkingLanguageAsync()).Id);

            var shipments = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).OrderBy(x => x.CreatedOnUtc).ToList();

            foreach (var shipment in shipments)
            {
                if (shipment.ShippedDateUtc.HasValue)
                    orderModel.ShipmentDate = await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);

                if (order.OrderStatus == OrderStatus.Processing)
                {
                    if (order.ShippingStatus == ShippingStatus.Shipped)
                    {
                        orderModel.OrderStatus = string.Format(
                            await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.ShipmentManagement.OrderStatus"),
                            order.ShippingMethod);

                    }
                    else if (order.ShippingStatus == ShippingStatus.NotYetShipped)
                    {
                        orderModel.OrderStatus = await _localizationService.GetResourceAsync(
                            "Delsami.Plugin.Misc.ShipmentManagement.Processing");
                    }
                }
                else if (order.OrderStatus == OrderStatus.Complete && order.ShippingStatus == ShippingStatus.Delivered)
                {
                    orderModel.OrderStatus = await _localizationService.GetResourceAsync(
                        "Delsami.Plugin.Misc.ShipmentManagement.Delivered");
                }

            }

            model.Orders.Add(orderModel);
        }

        var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(store.Id,
            customer.Id);
        foreach (var recurringPayment in recurringPayments)
        {
            var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);

            var recurringPaymentModel = new DelsamiCustomerOrderListModel.RecurringOrderModel
            {
                Id = recurringPayment.Id,
                StartDate = (await _dateTimeHelper.ConvertToUserTimeAsync(recurringPayment.StartDateUtc, DateTimeKind.Utc)).ToString(),
                CycleInfo = $"{recurringPayment.CycleLength} {await _localizationService.GetLocalizedEnumAsync(recurringPayment.CyclePeriod)}",
                NextPayment = await _orderProcessingService.GetNextPaymentDateAsync(recurringPayment) is DateTime nextPaymentDate ? (await _dateTimeHelper.ConvertToUserTimeAsync(nextPaymentDate, DateTimeKind.Utc)).ToString() : "",
                TotalCycles = recurringPayment.TotalCycles,
                CyclesRemaining = await _orderProcessingService.GetCyclesRemainingAsync(recurringPayment),
                InitialOrderId = order.Id,
                InitialOrderNumber = order.CustomOrderNumber,
                CanCancel = await _orderProcessingService.CanCancelRecurringPaymentAsync(customer, recurringPayment),
                CanRetryLastPayment = await _orderProcessingService.CanRetryLastRecurringPaymentAsync(customer, recurringPayment)
            };

            model.RecurringOrders.Add(recurringPaymentModel);
        }

        return model;
    }

    #endregion

    #region Utilities

    protected virtual async Task<PictureModel> PrepareOrderItemPictureModelAsync(OrderItem orderItem, int pictureSize, bool showDefaultPicture, string productName)
    {
        var language = await _workContext.GetWorkingLanguageAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var pictureCacheKey = _staticCacheManager.PrepareKeyForShortTermCache(NopModelCacheDefaults.OrderPictureModelKey,
            orderItem, pictureSize, showDefaultPicture, language, _webHelper.IsCurrentConnectionSecured(), store);

        var model = await _staticCacheManager.GetAsync(pictureCacheKey, async () =>
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            //order item picture
            var orderItemPicture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);

            return new PictureModel
            {
                ImageUrl = (await _pictureService.GetPictureUrlAsync(orderItemPicture, pictureSize, showDefaultPicture)).Url,
                Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"), productName),
                AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"), productName),
            };
        });

        return model;
    }

    #endregion
}
