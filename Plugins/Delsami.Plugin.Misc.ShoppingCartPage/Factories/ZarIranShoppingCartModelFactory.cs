using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.ShoppingCartPage.Models;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.Order;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using ZarIran.Plugin.Misc.ProductManagements.Services;

namespace ZarIran.Plugin.Misc.ShoppingCartPage.Factories
{
    public class ZarIranShoppingCartModelFactory
        : ShoppingCartModelFactory, IZarIranShoppingCartModelFactory
    {
        #region Fields

        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly CommonSettings _commonSettings;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly ITaxService _taxService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly MediaSettings _mediaSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly AddressSettings _addressSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ShippingSettings _shippingSettings;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IZarIranPriceCalculationService _zarIranPriceCalculationService;
        private readonly IProductPricingService _productPricingService;

        #endregion

        #region Ctor


        public ZarIranShoppingCartModelFactory(AddressSettings addressSettings,
                                              CaptchaSettings captchaSettings,
                                              CatalogSettings catalogSettings,
                                              CommonSettings commonSettings,
                                              CustomerSettings customerSettings,
                                              IAddressModelFactory addressModelFactory,
                                              ICheckoutAttributeFormatter checkoutAttributeFormatter,
                                              ICheckoutAttributeParser checkoutAttributeParser,
                                              ICheckoutAttributeService checkoutAttributeService,
                                              ICountryService countryService,
                                              ICurrencyService currencyService,
                                              ICustomerService customerService,
                                              IProductPricingService productPricingService,
                                              IDateTimeHelper dateTimeHelper,
                                              IDiscountService discountService,
                                              IOrderService orderService,
                                              IDownloadService downloadService,
                                              IGenericAttributeService genericAttributeService,
                                              IGiftCardService giftCardService,
                                              IHttpContextAccessor httpContextAccessor,
                                              ILocalizationService localizationService,
                                              IOrderProcessingService orderProcessingService,
                                              IOrderTotalCalculationService orderTotalCalculationService,
                                              IPaymentPluginManager paymentPluginManager,
                                              IPaymentService paymentService,
                                              IZarIranPriceCalculationService zarIranPriceCalculationService,
                                              IPermissionService permissionService,
                                              IPictureService pictureService,
                                              IPriceFormatter priceFormatter,
                                              IProductAttributeFormatter productAttributeFormatter,
                                              IProductService productService,
                                              IShippingService shippingService,
                                              IShoppingCartService shoppingCartService,
                                              IStateProvinceService stateProvinceService,
                                              IStaticCacheManager staticCacheManager,
                                              IStoreContext storeContext,
                                              IStoreMappingService storeMappingService,
                                              ITaxService taxService,
                                              IUrlRecordService urlRecordService,
                                              IVendorService vendorService,
                                              IWebHelper webHelper,
                                              IWorkContext workContext,
                                              MediaSettings mediaSettings,
                                              OrderSettings orderSettings,
                                              RewardPointsSettings rewardPointsSettings,
                                              ShippingSettings shippingSettings,
                                              ShoppingCartSettings shoppingCartSettings,
                                              TaxSettings taxSettings,
                                              VendorSettings vendorSettings) : base(addressSettings, captchaSettings, catalogSettings, commonSettings, customerSettings, addressModelFactory, checkoutAttributeFormatter, checkoutAttributeParser, checkoutAttributeService, countryService, currencyService, customerService, dateTimeHelper, discountService, downloadService, genericAttributeService, giftCardService, httpContextAccessor, localizationService, orderProcessingService, orderTotalCalculationService, paymentPluginManager, paymentService, permissionService, pictureService, priceFormatter, productAttributeFormatter, productService, shippingService, shoppingCartService, stateProvinceService, staticCacheManager, storeContext, storeMappingService, taxService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, orderSettings, rewardPointsSettings, shippingSettings, shoppingCartSettings, taxSettings, vendorSettings)
        {
            _orderTotalCalculationService = orderTotalCalculationService;
            _orderSettings = orderSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _catalogSettings = catalogSettings;
            _vendorSettings = vendorSettings;
            _workContext = workContext;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _commonSettings = commonSettings;
            _commonSettings = commonSettings;
            _productPricingService = productPricingService;
            _customerService = customerService;
            _discountService = discountService;
            _shoppingCartService = shoppingCartService;
            _priceFormatter = priceFormatter;
            _paymentPluginManager = paymentPluginManager;
            _checkoutAttributeService = checkoutAttributeService;
            _permissionService = permissionService;
            _taxService = taxService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _downloadService = downloadService;
            _productService = productService;
            _vendorService = vendorService;
            _urlRecordService = urlRecordService;
            _productAttributeFormatter = productAttributeFormatter;
            _mediaSettings = mediaSettings;
            _httpContextAccessor = httpContextAccessor;
            _addressModelFactory = addressModelFactory;
            _addressSettings = addressSettings;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _shippingSettings = shippingSettings;
            _pictureService = pictureService;
            _webHelper = webHelper;
            _orderService = orderService;
            _dateTimeHelper = dateTimeHelper;
            _zarIranPriceCalculationService = zarIranPriceCalculationService;
        }

        #endregion

        #region Methods

        public virtual async Task<ZarIranShoppingCartModel> PrepareShoppingCartModelAsync(ZarIranShoppingCartModel model,
                                                                                           IList<ShoppingCartItem> cart, bool isEditable = true,
                                                                                           bool validateCheckoutAttributes = false,
                                                                                           bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            //simple properties
            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            var orders = await _orderService.SearchOrdersAsync(storeId: store.Id,
                                                   customerId: customer.Id,
                                                   osIds: new List<int> { (int)OrderStatus.Pending });
            foreach (var order in orders)
            {
                var orderModel = new CustomerOrderListModel.OrderDetailsModel
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

                model.PendingOrders.Add(orderModel);
            }


            if (!cart.Any())
                return model;

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;

            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            var minOrderSubtotalAmountOk = await _orderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);
            if (!minOrderSubtotalAmountOk)
            {
                var minOrderSubtotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderSubtotalAmount, await _workContext.GetWorkingCurrencyAsync());
                model.MinOrderSubtotalWarning = string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderSubtotalAmount"), await _priceFormatter.FormatPriceAsync(minOrderSubtotalAmount, true, false));
            }

            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //discount and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCodes = await _customerService.ParseAppliedDiscountCouponCodesAsync(customer);
            foreach (var couponCode in discountCouponCodes)
            {
                var discount = await (await _discountService.GetAllDiscountsAsync(couponCode: couponCode))
                    .FirstOrDefaultAwaitAsync(async d => d.RequiresCouponCode && (await _discountService.ValidateDiscountAsync(d, customer)).IsValid);

                if (discount != null)
                {
                    model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    });
                }
            }

            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //checkout attributes
            model.CheckoutAttributes = await PrepareCheckoutAttributeModelsAsync(cart);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = await PrepareShoppingCartItemModelAsync(cart, sci);
                model.Items.Add(cartItemModel);
            }

            //payment methods
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = await (await _paymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id))
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            foreach (var pm in buttonPaymentMethods)
            {
                if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var viewComponent = pm.GetPublicViewComponent();
                model.ButtonPaymentMethodViewComponents.Add(viewComponent);
            }
            //hide "Checkout" button if we have only "Button" payment methods
            model.HideCheckoutButton = !nonButtonPaymentMethods.Any() && model.ButtonPaymentMethodViewComponents.Any();

            //order review data
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData = await PrepareOrderReviewDataModelAsync(cart);
            }

            return model;
        }


        public override async Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(IList<ShoppingCartItem> cart,
                                                                                  bool isEditable)
        {
            ArgumentNullException.ThrowIfNull(cart);

            var model = await base.PrepareOrderTotalsModelAsync(cart, isEditable);

            if (!cart.Any())
                return model;

            var customer = await _workContext.GetCurrentCustomerAsync();
            var currency = await _workContext.GetWorkingCurrencyAsync();

            decimal productsTotalInclTaxBase = 0m;
            var containsGoldProduct = false;

            foreach (var item in cart)
            {
                var pricingInfo = await _productPricingService
                    .GetByProductIdAsync(item.ProductId);

                if (pricingInfo != null)
                {
                    containsGoldProduct = true;

                    // همان سرویس استفاده‌شده در قیمت ردیف‌های سبد.
                    // قیمت خروجی، مالیات را هم در خودش دارد.
                    var (_, price) = await _zarIranPriceCalculationService
                        .GetCalculatedProductPriceAsync(item.ProductId);

                    productsTotalInclTaxBase += price * item.Quantity;
                }
                else
                {
                    // محصول معمولی: مسیر استاندارد ناپ.
                    var product = await _productService
                        .GetProductByIdAsync(item.ProductId);

                    var (subTotal, _, _, _) = await _shoppingCartService
                        .GetSubTotalAsync(item, true);

                    var (subTotalInclTax, _) = await _taxService
                        .GetProductPriceAsync(
                            product,
                            subTotal,
                            includingTax: true,
                            customer: customer);

                    productsTotalInclTaxBase += subTotalInclTax;
                }
            }

            if (!containsGoldProduct)
                return model;

            var productsTotal = await _currencyService
                .ConvertFromPrimaryStoreCurrencyAsync(
                    productsTotalInclTaxBase,
                    currency);

            model.SubTotal = await _priceFormatter
                .FormatPriceAsync(productsTotal);

            model.DisplayTax = false;
            model.DisplayTaxRates = false;
            model.TaxRates.Clear();

            decimal? shippingInclTaxBase = 0m;

            if (model.RequiresShipping)
            {
                var shipping = await _orderTotalCalculationService
                    .GetShoppingCartShippingTotalAsync(cart, true);

                shippingInclTaxBase = shipping.shippingTotal;

                if (shippingInclTaxBase.HasValue)
                {
                    var shippingAmount = await _currencyService
                        .ConvertFromPrimaryStoreCurrencyAsync(
                            shippingInclTaxBase.Value,
                            currency);

                    model.Shipping = await _priceFormatter
                        .FormatShippingPriceAsync(shippingAmount, true);
                }
            }

            if (shippingInclTaxBase.HasValue)
            {
                var total = await _currencyService
                    .ConvertFromPrimaryStoreCurrencyAsync(
                        productsTotalInclTaxBase + shippingInclTaxBase.Value,
                        currency);

                model.OrderTotal = await _priceFormatter
                    .FormatPriceAsync(total);
            }
            else
            {
                model.OrderTotal = null;
            }

            return model;
        }



        #endregion

        #region Utilties

        private async Task<ShoppingCartModel.ShoppingCartItemModel> PrepareShoppingCartItemModelAsync(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var product = await _productService.GetProductByIdAsync(sci.ProductId);
            var parentProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);

            var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = await _productService.FormatSkuAsync(product, sci.AttributesXml),
                VendorName = _vendorSettings.ShowVendorOnOrderDetailsPage ? (await _vendorService.GetVendorByProductIdAsync(product.Id))?.Name : string.Empty,
                ProductId = sci.ProductId,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(parentProduct),
                Quantity = sci.Quantity,
                AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml),
            };

            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             product.ProductType == ProductType.SimpleProduct &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              product.IsGiftCard) &&
                                             product.VisibleIndividually;

            //disable removal?
            //1. do other items require this one?
            cartItemModel.DisableRemoval = (await _shoppingCartService.GetProductsRequiringProductAsync(cart, product)).Any();

            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //recurring info
            if (product.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RecurringPeriod"),
                        product.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));

            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, sci.RentalStartDateUtc.Value)
                    : string.Empty;
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, sci.RentalEndDateUtc.Value)
                    : string.Empty;
                cartItemModel.RentalInfo =
                    string.Format(await _localizationService.GetResourceAsync("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

            var isCallForPrice = product.CallForPrice &&
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                 _workContext.OriginalCustomerIfImpersonated == null);

            // تشخیص محصول طلا
            var goldPricingInfo = await _productPricingService
                .GetByProductIdAsync(product.Id);

            var isGoldProduct = goldPricingInfo != null && !isCallForPrice;

            if (isCallForPrice)
            {
                var callForPriceText = await _localizationService
                    .GetResourceAsync("Products.CallForPrice");

                cartItemModel.UnitPrice = callForPriceText;
                cartItemModel.UnitPriceValue = 0;

                cartItemModel.SubTotal = callForPriceText;
                cartItemModel.SubTotalValue = 0;
            }
            else if (isGoldProduct)
            {
                // قیمت لحظه‌ای طلا به ارز پایه فروشگاه
                var (_, calculatedPrice) =
                    await _zarIranPriceCalculationService
                        .GetCalculatedProductPriceAsync(product.Id);

                // قیمت واحد
                var unitPrice = await _currencyService
                    .ConvertFromPrimaryStoreCurrencyAsync(
                        calculatedPrice, currentCurrency);

                cartItemModel.UnitPrice =
                    await _priceFormatter.FormatPriceAsync(unitPrice);

                cartItemModel.UnitPriceValue = unitPrice;

                // جمع قیمت بر اساس تعداد
                var subTotalBase = calculatedPrice * sci.Quantity;

                var subTotal = await _currencyService
                    .ConvertFromPrimaryStoreCurrencyAsync(
                        subTotalBase, currentCurrency);

                cartItemModel.SubTotal =
                    await _priceFormatter.FormatPriceAsync(subTotal);

                cartItemModel.SubTotalValue = subTotal;

                // تخفیف‌های استاندارد ناپ در این شاخه اعمال نشده‌اند
                cartItemModel.MaximumDiscountedQty = null;
                cartItemModel.DiscountValue = 0;
            }
            else
            {
                // رفتار اصلی nopCommerce 4.60
                var (shoppingCartUnitPriceWithDiscountBase, _) =
                    await _taxService.GetProductPriceAsync(
                        product,
                        (await _shoppingCartService
                            .GetUnitPriceAsync(sci, true)).unitPrice);

                var shoppingCartUnitPriceWithDiscount =
                    await _currencyService
                        .ConvertFromPrimaryStoreCurrencyAsync(
                            shoppingCartUnitPriceWithDiscountBase,
                            currentCurrency);

                cartItemModel.UnitPrice =
                    await _priceFormatter.FormatPriceAsync(
                        shoppingCartUnitPriceWithDiscount);

                cartItemModel.UnitPriceValue =
                    shoppingCartUnitPriceWithDiscount;

                var (subTotal, shoppingCartItemDiscountBase,
                     _, maximumDiscountQty) =
                    await _shoppingCartService
                        .GetSubTotalAsync(sci, true);

                var (shoppingCartItemSubTotalWithDiscountBase, _) =
                    await _taxService.GetProductPriceAsync(
                        product, subTotal);

                var shoppingCartItemSubTotalWithDiscount =
                    await _currencyService
                        .ConvertFromPrimaryStoreCurrencyAsync(
                            shoppingCartItemSubTotalWithDiscountBase,
                            currentCurrency);

                cartItemModel.SubTotal =
                    await _priceFormatter.FormatPriceAsync(
                        shoppingCartItemSubTotalWithDiscount);

                cartItemModel.SubTotalValue =
                    shoppingCartItemSubTotalWithDiscount;

                cartItemModel.MaximumDiscountedQty =
                    maximumDiscountQty;

                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    (shoppingCartItemDiscountBase, _) =
                        await _taxService.GetProductPriceAsync(
                            product, shoppingCartItemDiscountBase);

                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount =
                            await _currencyService
                                .ConvertFromPrimaryStoreCurrencyAsync(
                                    shoppingCartItemDiscountBase,
                                    currentCurrency);

                        cartItemModel.Discount =
                            await _priceFormatter.FormatPriceAsync(
                                shoppingCartItemDiscount);

                        cartItemModel.DiscountValue =
                            shoppingCartItemDiscount;
                    }
                }
            }


            //picture
            if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
            {
                cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            //item warnings
            var itemWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(
                await _workContext.GetCurrentCustomerAsync(),
                sci.ShoppingCartType,
                product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        private async Task<ShoppingCartModel.OrderReviewDataModel> PrepareOrderReviewDataModelAsync(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new ShoppingCartModel.OrderReviewDataModel
            {
                Display = true
            };

            //billing info
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
            if (billingAddress != null)
            {
                await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress,
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
            }

            //shipping info
            if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                model.IsShippable = true;

                var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(customer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
                model.SelectedPickupInStore = _shippingSettings.AllowPickupInStore && pickupPoint != null;
                if (!model.SelectedPickupInStore)
                {
                    if (await _customerService.GetCustomerShippingAddressAsync(customer) is Address address)
                    {
                        await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress,
                            address: address,
                            excludeProperties: false,
                            addressSettings: _addressSettings);
                    }
                }
                else
                {
                    var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
                    var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);

                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        County = pickupPoint.County,
                        CountryName = country?.Name ?? string.Empty,
                        StateProvinceName = state?.Name ?? string.Empty,
                        ZipPostalCode = pickupPoint.ZipPostalCode
                    };
                }

                //selected shipping method
                var shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
                if (shippingOption != null)
                    model.ShippingMethod = shippingOption.Name;
            }

            //payment info
            var selectedPaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await _paymentPluginManager
                .LoadPluginBySystemNameAsync(selectedPaymentMethodSystemName, customer, store.Id);
            model.PaymentMethod = paymentMethod != null
                ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, (await _workContext.GetWorkingLanguageAsync()).Id)
                : string.Empty;

            //custom values
            var processPaymentRequest = _httpContextAccessor.HttpContext?.Session?.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            if (processPaymentRequest != null)
                model.CustomValues = processPaymentRequest.CustomValues;

            return model;
        }

        private async Task<PictureModel> PrepareCartItemPictureModelAsync(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = _staticCacheManager.PrepareKeyForShortTermCache(NopModelCacheDefaults.CartPictureModelKey
                , sci, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

            var model = await _staticCacheManager.GetAsync(pictureCacheKey, async () =>
            {
                var product = await _productService.GetProductByIdAsync(sci.ProductId);

                //shopping cart item picture
                var sciPicture = await _pictureService.GetProductPictureAsync(product, sci.AttributesXml);

                return new PictureModel
                {
                    ImageUrl = (await _pictureService.GetPictureUrlAsync(sciPicture, pictureSize, showDefaultPicture)).Url,
                    Title = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageLinkTitleFormat"), productName),
                    AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Product.ImageAlternateTextFormat"), productName),
                };
            });

            return model;
        }

        private async Task<IList<ShoppingCartModel.CheckoutAttributeModel>> PrepareCheckoutAttributeModelsAsync(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new List<ShoppingCartModel.CheckoutAttributeModel>();
            var store = await _storeContext.GetCurrentStoreAsync();
            var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var checkoutAttributes =
                await _checkoutAttributeService.GetAllCheckoutAttributesAsync(store.Id,
                    excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                    TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue)
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _checkoutAttributeService.GetCheckoutAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
                        {
                            var (priceAdjustmentBase, _) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue);
                            var priceAdjustment =
                                await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase,
                                    await _workContext.GetWorkingCurrencyAsync());
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "+" + await _priceFormatter.FormatPriceAsync(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "-" + await _priceFormatter.FormatPriceAsync(-priceAdjustment);
                        }
                    }
                }

                //set already selected attributes
                var selectedCheckoutAttributes = await _genericAttributeService.GetAttributeAsync<string>(
                    await _workContext.GetCurrentCustomerAsync(),
                    NopCustomerDefaults.CheckoutAttributes, store.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues =
                                    _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var attributeValue in await selectedValues.SelectMany(x => x.values).ToListAsync())
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText =
                                    _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }

                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr =
                                _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (selectedDateStr.Any())
                            {
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                    DateTimeStyles.None, out var selectedDate))
                                {
                                    //successfully parsed
                                    attributeModel.SelectedDay = selectedDate.Day;
                                    attributeModel.SelectedMonth = selectedDate.Month;
                                    attributeModel.SelectedYear = selectedDate.Year;
                                }
                            }
                        }

                        break;
                    case AttributeControlType.FileUpload:
                        {
                            if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var downloadGuidStr = _checkoutAttributeParser
                                    .ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                                _ = Guid.TryParse(downloadGuidStr, out var downloadGuid);
                                var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                                if (download != null)
                                    attributeModel.DefaultValue = download.DownloadGuid.ToString();
                            }
                        }

                        break;
                    default:
                        break;
                }

                model.Add(attributeModel);
            }

            return model;
        }

        #endregion
    }
}
