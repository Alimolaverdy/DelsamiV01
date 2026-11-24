using ZarIran.Plugin.Misc.ExtendedOrder;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using ZarIran.Plugin.Misc.Core.Events;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using ZarIran.Plugin.Misc.ExtendedOrder;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class ZarIranOrderProcessingService : OrderProcessingService, IOrderProcessingService
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITaxService _taxService;
        protected readonly IDiscountService _discountService;
        private readonly IStoreService _storeService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IShippingService _shippingService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IOrderService _orderService;
        private readonly ILocalizationService _localizationService;
        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly IReservedStockQuantityService _reservedStockQuantityService;
        private readonly OrderSettings _orderSettings;
        private readonly ExtendedOrderSettings _extendedOrderSettings;
        private readonly IGiftCardService _giftCardService;
        private readonly ISubStatusMappingService _subStatusMappingService;
        private readonly ISubStatusService _subStatusService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public ZarIranOrderProcessingService(CurrencySettings currencySettings,
                                              IAddressService addressService,
                                              IAffiliateService affiliateService,
                                              ICheckoutAttributeFormatter checkoutAttributeFormatter,
                                              ICountryService countryService,
                                              ICurrencyService currencyService,
                                              ICustomerActivityService customerActivityService,
                                              ICustomerService customerService,
                                              ICustomNumberFormatter customNumberFormatter,
                                              IDiscountService discountService,
                                              IEncryptionService encryptionService,
                                              IEventPublisher eventPublisher,
                                              IGenericAttributeService genericAttributeService,
                                              IGiftCardService giftCardService,
                                              ILanguageService languageService,
                                              ILocalizationService localizationService,
                                              ILogger logger,
                                              IOrderService orderService,
                                              IOrderTotalCalculationService orderTotalCalculationService,
                                              IPaymentPluginManager paymentPluginManager,
                                              IPaymentService paymentService,
                                              IPdfService pdfService,
                                              IPriceCalculationService priceCalculationService,
                                              IPriceFormatter priceFormatter,
                                              IProductAttributeFormatter productAttributeFormatter,
                                              IProductAttributeParser productAttributeParser,
                                              IProductService productService,
                                              IReturnRequestService returnRequestService,
                                              IRewardPointService rewardPointService,
                                              IShipmentService shipmentService,
                                              IShippingService shippingService,
                                              IShoppingCartService shoppingCartService,
                                              IStateProvinceService stateProvinceService,
                                              IStoreService storeService,
                                              ITaxService taxService,
                                              IVendorService vendorService,
                                              IWebHelper webHelper,
                                              IWorkContext workContext,
                                              IWorkflowMessageService workflowMessageService,
                                              LocalizationSettings localizationSettings,
                                              OrderSettings orderSettings,
                                              PaymentSettings paymentSettings,
                                              RewardPointsSettings rewardPointsSettings,
                                              ShippingSettings shippingSettings,
                                              TaxSettings taxSettings,
                                              IVirtualProductMappingService virtualProductMappingService,
                                              IReservedStockQuantityService reservedStockQuantityService,
                                              ISubStatusMappingService subStatusMappingService,
                                              ISubStatusService subStatusService,
                                              ExtendedOrderSettings extendedOrderSettings) : base(currencySettings,
                                                                              addressService,
                                                                              affiliateService,
                                                                              checkoutAttributeFormatter,
                                                                              countryService,
                                                                              currencyService,
                                                                              customerActivityService,
                                                                              customerService,
                                                                              customNumberFormatter,
                                                                              discountService,
                                                                              encryptionService,
                                                                              eventPublisher,
                                                                              genericAttributeService,
                                                                              giftCardService,
                                                                              languageService,
                                                                              localizationService,
                                                                              logger,
                                                                              orderService,
                                                                              orderTotalCalculationService,
                                                                              paymentPluginManager,
                                                                              paymentService,
                                                                              pdfService,
                                                                              priceCalculationService,
                                                                              priceFormatter,
                                                                              productAttributeFormatter,
                                                                              productAttributeParser,
                                                                              productService,
                                                                              returnRequestService,
                                                                              rewardPointService,
                                                                              shipmentService,
                                                                              shippingService,
                                                                              shoppingCartService,
                                                                              stateProvinceService,
                                                                              storeService,
                                                                              taxService,
                                                                              vendorService,
                                                                              webHelper,
                                                                              workContext,
                                                                              workflowMessageService,
                                                                              localizationSettings,
                                                                              orderSettings,
                                                                              paymentSettings,
                                                                              rewardPointsSettings,
                                                                              shippingSettings,
                                                                              taxSettings)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _taxService = taxService;
            _discountService = discountService;
            _storeService = storeService;
            _productAttributeFormatter = productAttributeFormatter;
            _shippingService = shippingService;
            _priceCalculationService = priceCalculationService;
            _orderService = orderService;
            _localizationService = localizationService;
            _virtualProductMappingService = virtualProductMappingService;
            _reservedStockQuantityService = reservedStockQuantityService;
            _orderSettings = orderSettings;
            _giftCardService = giftCardService;
            _subStatusMappingService = subStatusMappingService;
            _subStatusService = subStatusService;
            _extendedOrderSettings = extendedOrderSettings;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        protected override async Task MoveShoppingCartItemsToOrderItemsAsync(PlaceOrderContainer details, Order order)
        {
            foreach (var sc in details.Cart)
            {
                var product = await _productService.GetProductByIdAsync(sc.ProductId);

                //prices
                var scUnitPrice = (await _shoppingCartService.GetUnitPriceAsync(sc, true)).unitPrice;
                var (scSubTotal, discountAmount, scDiscounts, _) = await _shoppingCartService.GetSubTotalAsync(sc, true);
                var scUnitPriceInclTax = await _taxService.GetProductPriceAsync(product, scUnitPrice, true, details.Customer);
                var scUnitPriceExclTax = await _taxService.GetProductPriceAsync(product, scUnitPrice, false, details.Customer);
                var scSubTotalInclTax = await _taxService.GetProductPriceAsync(product, scSubTotal, true, details.Customer);
                var scSubTotalExclTax = await _taxService.GetProductPriceAsync(product, scSubTotal, false, details.Customer);
                var discountAmountInclTax = await _taxService.GetProductPriceAsync(product, discountAmount, true, details.Customer);
                var discountAmountExclTax = await _taxService.GetProductPriceAsync(product, discountAmount, false, details.Customer);
                foreach (var disc in scDiscounts)
                    if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                        details.AppliedDiscounts.Add(disc);

                //attributes
                var store = await _storeService.GetStoreByIdAsync(sc.StoreId);
                var attributeDescription = await _productAttributeFormatter.FormatAttributesAsync(product, sc.AttributesXml, details.Customer, store);

                var itemWeight = await _shippingService.GetShoppingCartItemWeightAsync(sc);

                //save order item
                var orderItem = new OrderItem
                {
                    OrderItemGuid = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    UnitPriceInclTax = scUnitPriceInclTax.price,
                    UnitPriceExclTax = scUnitPriceExclTax.price,
                    PriceInclTax = scSubTotalInclTax.price,
                    PriceExclTax = scSubTotalExclTax.price,
                    OriginalProductCost = await _priceCalculationService.GetProductCostAsync(product, sc.AttributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = sc.AttributesXml,
                    Quantity = sc.Quantity,
                    DiscountAmountInclTax = discountAmountInclTax.price,
                    DiscountAmountExclTax = discountAmountExclTax.price,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    ItemWeight = itemWeight,
                    RentalStartDateUtc = sc.RentalStartDateUtc,
                    RentalEndDateUtc = sc.RentalEndDateUtc
                };

                await _orderService.InsertOrderItemAsync(orderItem);

                //gift cards
                await AddGiftCardsAsync(product, sc.AttributesXml, sc.Quantity, orderItem, scUnitPriceExclTax.price);

                //var mainProductId = (await _virtualProductMappingService.GetVirtualProductMappingByVirtualProductIdAsync(product.Id))?.MainProductId;

                //if (mainProductId.HasValue)
                //{
                //    var mainProduct = await _productService.GetProductByIdAsync(mainProductId.Value);

                await _reservedStockQuantityService.AdjustReservedStockQuantityAsync(product, order.Id.ToString(), sc.Quantity, string.Format(await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ProductManagements.Admin.ReservedStockQuantityHistory.Messages.PlaceOrder"), order.Id));
                //}
                //else
                //{
                //    //inventory
                //    await _reservedStockQuantityService.AdjustReservedStockQuantityAsync(product, order.Id, -sc.Quantity);
                //}
            }

            //clear shopping cart
            await Task.WhenAll(details.Cart.ToList().Select(sci => _shoppingCartService.DeleteShoppingCartItemAsync(sci, false)));
        }

        public override async Task CancelOrderAsync(Order order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanCancelOrder(order))
                throw new NopException("Cannot do cancel for order.");

            var previousOrderStatus = order.OrderStatus;

            //cancel order
            await SetOrderStatusAsync(order, OrderStatus.Cancelled, notifyCustomer);

            //add a note
            await AddOrderNoteAsync(order, "Order has been cancelled");

            //return (add) back redeemded reward points
            await ReturnBackRedeemedRewardPointsAsync(order);

            //delete gift card usage history
            if (_orderSettings.DeleteGiftCardUsageHistory)
                await _giftCardService.DeleteGiftCardUsageHistoryAsync(order);

            //cancel recurring payments
            var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id);
            foreach (var rp in recurringPayments)
                await CancelRecurringPaymentAsync(rp);

            if (previousOrderStatus == OrderStatus.Pending)
            {
                await _reservedStockQuantityService.ReverseReservedStockQuantityAsync(order, string.Format(await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ProductManagements.Admin.ReservedStockQuantityHistory.Messages.PendingOrderCanceled"), order.Id));
            }
            else
            {
                //Adjust inventory for already shipped shipments
                //only products with "use multiple warehouses"
                await ReverseBookedInventoryAsync(order, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));

                //Adjust inventory
                await ReturnOrderStockAsync(order, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));

                var orderCanceledEvent = new OrderCanceledEvent(order);

                _eventPublisher.Publish(orderCanceledEvent);
            }
        }
        #endregion
    }
}
