using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.ProductManagements.Services;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
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
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Factories
{
    internal class ZarIranShoppingCartModelFactory : ShoppingCartModelFactory
    {
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly VendorSettings _vendorSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly MediaSettings _mediaSettings;
        private readonly IVirtualProductMappingService _virtualProductMappingService;

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
                                                IDateTimeHelper dateTimeHelper,
                                                IDiscountService discountService,
                                                IDownloadService downloadService,
                                                IGenericAttributeService genericAttributeService,
                                                IGiftCardService giftCardService,
                                                IHttpContextAccessor httpContextAccessor,
                                                ILocalizationService localizationService,
                                                IOrderProcessingService orderProcessingService,
                                                IOrderTotalCalculationService orderTotalCalculationService,
                                                IPaymentPluginManager paymentPluginManager,
                                                IPaymentService paymentService,
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
                                                VendorSettings vendorSettings,
                                                IVirtualProductMappingService virtualProductMappingService) : base(addressSettings,
                                                                                      captchaSettings,
                                                                                      catalogSettings,
                                                                                      commonSettings,
                                                                                      customerSettings,
                                                                                      addressModelFactory,
                                                                                      checkoutAttributeFormatter,
                                                                                      checkoutAttributeParser,
                                                                                      checkoutAttributeService,
                                                                                      countryService,
                                                                                      currencyService,
                                                                                      customerService,
                                                                                      dateTimeHelper,
                                                                                      discountService,
                                                                                      downloadService,
                                                                                      genericAttributeService,
                                                                                      giftCardService,
                                                                                      httpContextAccessor,
                                                                                      localizationService,
                                                                                      orderProcessingService,
                                                                                      orderTotalCalculationService,
                                                                                      paymentPluginManager,
                                                                                      paymentService,
                                                                                      permissionService,
                                                                                      pictureService,
                                                                                      priceFormatter,
                                                                                      productAttributeFormatter,
                                                                                      productService,
                                                                                      shippingService,
                                                                                      shoppingCartService,
                                                                                      stateProvinceService,
                                                                                      staticCacheManager,
                                                                                      storeContext,
                                                                                      storeMappingService,
                                                                                      taxService,
                                                                                      urlRecordService,
                                                                                      vendorService,
                                                                                      webHelper,
                                                                                      workContext,
                                                                                      mediaSettings,
                                                                                      orderSettings,
                                                                                      rewardPointsSettings,
                                                                                      shippingSettings,
                                                                                      shoppingCartSettings,
                                                                                      taxSettings,
                                                                                      vendorSettings)
        {
            _productService = productService;
            _vendorService = vendorService;
            _vendorSettings = vendorSettings;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _productAttributeFormatter = productAttributeFormatter;
            _shoppingCartSettings = shoppingCartSettings;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _taxService = taxService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _mediaSettings = mediaSettings;
            _virtualProductMappingService = virtualProductMappingService;
        }

        protected override async Task<ShoppingCartModel.ShoppingCartItemModel> PrepareShoppingCartItemModelAsync(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var product = await _productService.GetProductByIdAsync(sci.ProductId);

            var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = await _productService.FormatSkuAsync(product, sci.AttributesXml),
                VendorName = _vendorSettings.ShowVendorOnOrderDetailsPage ? (await _vendorService.GetVendorByProductIdAsync(product.Id))?.Name : string.Empty,
                ProductId = sci.ProductId,
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                Quantity = sci.Quantity,
                AttributeInfo = await _productAttributeFormatter.FormatAttributesAsync(product, sci.AttributesXml),
            };

            var mainProductResult = await _virtualProductMappingService.GetMainProductByVirtualProductIdAsync(product.Id);

            var isVirtualProduct = mainProductResult.IsVirtualPrdocut;
            var mainProduct = mainProductResult.MainProduct;

            //if (isVirtualProduct && mainProduct is null)
            //    return null;

            product = mainProduct ?? product;

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
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.UnitPrice = await _localizationService.GetResourceAsync("Products.CallForPrice");
                cartItemModel.UnitPriceValue = 0;
            }
            else
            {
                var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
                cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
            }
            //subtotal, discount
            if (product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.SubTotal = await _localizationService.GetResourceAsync("Products.CallForPrice");
                cartItemModel.SubTotalValue = 0;
            }
            else
            {
                //sub total
                var (subTotal, shoppingCartItemDiscountBase, _, maximumDiscountQty) = await _shoppingCartService.GetSubTotalAsync(sci, true);
                var (shoppingCartItemSubTotalWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, subTotal);
                var shoppingCartItemSubTotalWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
                cartItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    (shoppingCartItemDiscountBase, _) = await _taxService.GetProductPriceAsync(product, shoppingCartItemDiscountBase);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemDiscountBase, currentCurrency);
                        cartItemModel.Discount = await _priceFormatter.FormatPriceAsync(shoppingCartItemDiscount);
                        cartItemModel.DiscountValue = shoppingCartItemDiscount;
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
    }
}
