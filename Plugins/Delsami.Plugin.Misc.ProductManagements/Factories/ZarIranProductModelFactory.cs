using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.Core.Extensions;
using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.ProductManagements.Models;
using ZarIran.Plugin.Misc.ProductManagements.Services;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Factories
{
    public class ZarIranProductModelFactory : ProductModelFactory
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly IPermissionService _permissionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly OrderSettings _orderSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly SeoSettings _seoSettings;
        private readonly IDateRangeService _dateRangeService;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IProductAttributeService _productAttributeService;
        private readonly MediaSettings _mediaSettings;
        private readonly ICustomerService _customerService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ShippingSettings _shippingSettings;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IReservedStockQuantityService _reservedQuantityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IBundleProductService _bundleProductService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IPictureService _pictureService;
        private readonly IManufacturerService _manufacturerService;
        private readonly CoreSettings _coreSettings;
        private readonly IDiscountService _discountService;
        private readonly ISettingService _settingService;
        private readonly IProductPricingService _productPricingService;
        private readonly IZarIranPriceCalculationService _zarIranPriceCalculationService;

        #endregion

        #region Ctor
        public ZarIranProductModelFactory(CaptchaSettings captchaSettings,
                                           CatalogSettings catalogSettings,
                                           CustomerSettings customerSettings,
                                           ICategoryService categoryService,
                                           ICurrencyService currencyService,
                                           ICustomerService customerService,
                                           IDateRangeService dateRangeService,
                                           IDateTimeHelper dateTimeHelper,
                                           IDownloadService downloadService,
                                           IGenericAttributeService genericAttributeService,
                                           ILocalizationService localizationService,
                                           IManufacturerService manufacturerService,
                                           IPermissionService permissionService,
                                           IPictureService pictureService,
                                           IPriceCalculationService priceCalculationService,
                                           IPriceFormatter priceFormatter,
                                           IProductAttributeParser productAttributeParser,
                                           IProductAttributeService productAttributeService,
                                           IProductService productService,
                                           IProductTagService productTagService,
                                           IProductTemplateService productTemplateService,
                                           IReviewTypeService reviewTypeService,
                                           IShoppingCartService shoppingCartService,
                                           ISpecificationAttributeService specificationAttributeService,
                                           IStaticCacheManager staticCacheManager,
                                           IStoreContext storeContext,
                                           IStoreService storeService,
                                           IShoppingCartModelFactory shoppingCartModelFactory,
                                           ITaxService taxService,
                                           IUrlRecordService urlRecordService,
                                           IVendorService vendorService,
                                           IVideoService videoService,
                                           IWebHelper webHelper,
                                           IWorkContext workContext,
                                           IZarIranPriceCalculationService zarIranPriceCalculationService,
                                           MediaSettings mediaSettings,
                                           OrderSettings orderSettings,
                                           SeoSettings seoSettings,
                                           ShippingSettings shippingSettings,
                                           VendorSettings vendorSettings,
                                           IVirtualProductMappingService virtualProductMappingService,
                                           IReservedStockQuantityService reservedQuantityService,
                                           IBundleProductService bundleProductService,
                                           CoreSettings coreSettings,
                                           IDiscountService discountService,
                                           ISettingService settingService,
                                           IProductPricingService productPricingService) : base(captchaSettings,
                                                                                                              catalogSettings,
                                                                                                              customerSettings,
                                                                                                              categoryService,
                                                                                                              currencyService,
                                                                                                              customerService,
                                                                                                              dateRangeService,
                                                                                                              dateTimeHelper,
                                                                                                              downloadService,
                                                                                                              genericAttributeService,
                                                                                                              localizationService,
                                                                                                              manufacturerService,
                                                                                                              permissionService,
                                                                                                              pictureService,
                                                                                                              priceCalculationService,
                                                                                                              priceFormatter,
                                                                                                              productAttributeParser,
                                                                                                              productAttributeService,
                                                                                                              productService,
                                                                                                              productTagService,
                                                                                                              productTemplateService,
                                                                                                              reviewTypeService,
                                                                                                              shoppingCartService,
                                                                                                              specificationAttributeService,
                                                                                                              staticCacheManager,
                                                                                                              storeContext,
                                                                                                              storeService,
                                                                                                              shoppingCartModelFactory,
                                                                                                              taxService,
                                                                                                              urlRecordService,
                                                                                                              vendorService,
                                                                                                              videoService,
                                                                                                              webHelper,
                                                                                                              workContext,
                                                                                                              mediaSettings,
                                                                                                              orderSettings,
                                                                                                              seoSettings,
                                                                                                              shippingSettings,
                                                                                                              vendorSettings)
        {
            _storeContext = storeContext;
            _productService = productService;
            _permissionService = permissionService;
            _catalogSettings = catalogSettings;
            _workContext = workContext;
            _priceCalculationService = priceCalculationService;
            _orderSettings = orderSettings;
            _localizationService = localizationService;
            _taxService = taxService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _virtualProductMappingService = virtualProductMappingService;
            _urlRecordService = urlRecordService;
            _seoSettings = seoSettings;
            _dateRangeService = dateRangeService;
            _vendorSettings = vendorSettings;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _productAttributeService = productAttributeService;
            _mediaSettings = mediaSettings;
            _customerService = customerService;
            _productAttributeParser = productAttributeParser;
            _shippingSettings = shippingSettings;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _reservedQuantityService = reservedQuantityService;
            _dateTimeHelper = dateTimeHelper;
            _bundleProductService = bundleProductService;
            _staticCacheManager = staticCacheManager;
            _pictureService = pictureService;
            _manufacturerService = manufacturerService;
            _coreSettings = coreSettings;
            _discountService = discountService;
            _settingService = settingService;
            _zarIranPriceCalculationService = zarIranPriceCalculationService;
            _productPricingService = productPricingService;
        }

        #endregion

        #region Methods

        protected override async Task PrepareGroupedProductOverviewPriceModelAsync(Product product, ProductOverviewModel.ProductPriceModel priceModel)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id,
                store.Id);
            //add to cart button (ignore "DisableBuyButton" property for grouped products)
            priceModel.DisableBuyButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);
            //add to wishlist button (ignore "DisableWishlistButton" property for grouped products)
            priceModel.DisableWishlistButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);
            //compare products
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
            if (!associatedProducts.Any() ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
                return;

            var hasAvailableProduct = false;
            Product lowestPricedProduct = null;
            decimal lowestPrice = 0;
            decimal previousPrice = 0;

            foreach (var associatedProduct in associatedProducts)
            {
                var mapping = await _virtualProductMappingService.GetMainProductByVirtualProductIdAsync(associatedProduct.Id);
                var pricedProduct = mapping.IsVirtualPrdocut ? mapping.MainProduct : associatedProduct;
                if (pricedProduct == null || pricedProduct.Deleted || !pricedProduct.Published || pricedProduct.DisableBuyButton ||
                    !await IsAvailableForGroupedPriceAsync(pricedProduct))
                    continue;

                hasAvailableProduct = true;
                if (pricedProduct.ProductType != ProductType.SimpleProduct || pricedProduct.CustomerEntersPrice ||
                    await _productPricingService.GetByProductIdAsync(pricedProduct.Id) == null)
                    continue;

                // Match product details: the ZarIran service returns tax-inclusive prices for a simple product.
                var (calculatedOldPrice, calculatedPrice) =
                    await _zarIranPriceCalculationService.GetCalculatedProductPriceAsync(pricedProduct.Id);
                if (calculatedPrice <= 0 || (lowestPricedProduct != null && calculatedPrice >= lowestPrice))
                    continue;

                lowestPricedProduct = pricedProduct;
                lowestPrice = calculatedPrice;
                previousPrice = calculatedOldPrice;
            }

            if (!hasAvailableProduct)
            {
                priceModel.Price = await _localizationService.GetResourceAsync("Products.Availability.OutOfStock");
                return;
            }

            if (lowestPricedProduct == null)
                return;

            if (lowestPricedProduct.CallForPrice &&
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts ||
                 _workContext.OriginalCustomerIfImpersonated == null))
            {
                priceModel.Price = await _localizationService.GetResourceAsync("Products.CallForPrice");
                return;
            }

            var currency = await _workContext.GetWorkingCurrencyAsync();
            var fromPrice = await _localizationService.GetResourceAsync("Products.PriceRangeFrom");
            var finalPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(lowestPrice, currency);
            priceModel.PriceValue = finalPrice;
            priceModel.Price = string.Format(fromPrice, await _priceFormatter.FormatPriceAsync(finalPrice));

            if (previousPrice > lowestPrice)
            {
                var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(previousPrice, currency);
                priceModel.OldPriceValue = oldPrice;
                priceModel.OldPrice = string.Format(fromPrice, await _priceFormatter.FormatPriceAsync(oldPrice));
            }

            priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(lowestPricedProduct, lowestPrice);
            priceModel.BasePricePAngVValue = lowestPrice;
        }

        private async Task<bool> IsAvailableForGroupedPriceAsync(Product product)
        {
            return product.ManageInventoryMethod switch
            {
                ManageInventoryMethod.DontManageStock => true,
                ManageInventoryMethod.ManageStock => product.BackorderMode != BackorderMode.NoBackorders ||
                    await _productService.GetTotalStockQuantityAsync(product) >
                    await _reservedQuantityService.GetReservedStockQuantityCountAsync(product.Id),
                ManageInventoryMethod.ManageStockByAttributes => (await _productAttributeService
                    .GetAllProductAttributeCombinationsAsync(product.Id))
                    .Any(combination => combination.StockQuantity > 0 || combination.AllowOutOfStockOrders),
                _ => false
            };
        }

        public override async Task<ProductDetailsModel> PrepareProductDetailsModelAsync(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            Product mainProduct = null;
            bool isVirtualProduct = false;

            if (isAssociatedProduct)
            {
                var mainProductResult = await _virtualProductMappingService.GetMainProductByVirtualProductIdAsync(product.Id);

                isVirtualProduct = mainProductResult.IsVirtualPrdocut;
                mainProduct = mainProductResult.MainProduct;

                if (isVirtualProduct && mainProduct is null)
                    return null;
            }

            //standard properties
            var model = new ProductDetailsModel
            {
                Id = product.Id,
                Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, ensureTwoPublishedLanguages: false),
                MetaKeywords = await _localizationService.GetLocalizedAsync(product, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(product, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(product, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(product),
                ProductType = product.ProductType,
                Weight = (mainProduct ?? product).Weight,
                ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage,
                Sku = product.Sku,
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = product.Gtin,
                ManageInventoryMethod = product.ManageInventoryMethod,
                StockAvailability = await _productService.FormatStockMessageAsync(mainProduct ?? product, string.Empty),
                HasSampleDownload = product.IsDownload && product.HasSampleDownload,
                DisplayDiscontinuedMessage = !product.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedProducts,
                AvailableEndDate = product.AvailableEndDateTimeUtc,
                VisibleIndividually = product.VisibleIndividually,
                AllowAddingOnlyExistingAttributeCombinations = product.AllowAddingOnlyExistingAttributeCombinations
            };

            //automatically generate product description?
            if (_seoSettings.GenerateProductMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //shipping info
            model.IsShipEnabled = product.IsShipEnabled;
            if (product.IsShipEnabled)
            {
                model.IsFreeShipping = product.IsFreeShipping;
                //delivery date
                var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(product.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = await _localizationService.GetLocalizedAsync(deliveryDate, dd => dd.Name);
                }
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare products
            model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;
            //store name
            model.CurrentStoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name);

            //vendor details
            if (_vendorSettings.ShowVendorOnProductDetailsPage)
            {
                var vendor = await _vendorService.GetVendorByIdAsync(product.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;

                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(vendor),
                    };
                }
            }

            //page sharing
            if (_catalogSettings.ShowShareButton && !string.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the add this link to be https linked when the page is, so that the page doesn't ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }

                model.PageShareCode = shareCode;
            }

            if (mainProduct != null)
            {
                switch (mainProduct.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        model.InStock = true;
                        break;

                    case ManageInventoryMethod.ManageStock:
                        model.InStock = mainProduct.BackorderMode != BackorderMode.NoBackorders
                            || await _productService.GetTotalStockQuantityAsync(mainProduct) - (await _reservedQuantityService.GetReservedStockQuantityCountAsync(mainProduct.Id)) > 0;
                        model.DisplayBackInStockSubscription = !model.InStock && mainProduct.AllowBackInStockSubscriptions;
                        break;

                    case ManageInventoryMethod.ManageStockByAttributes:
                        model.InStock = (await _productAttributeService
                            .GetAllProductAttributeCombinationsAsync(mainProduct.Id))
                            ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                            ?? false;
                        break;
                }
            }
            else
            {
                switch (product.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        model.InStock = true;
                        break;

                    case ManageInventoryMethod.ManageStock:
                        model.InStock = product.BackorderMode != BackorderMode.NoBackorders
                            || await _productService.GetTotalStockQuantityAsync(product) - (await _reservedQuantityService.GetReservedStockQuantityCountAsync(product.Id)) > 0;
                        model.DisplayBackInStockSubscription = !model.InStock && product.AllowBackInStockSubscriptions;
                        break;

                    case ManageInventoryMethod.ManageStockByAttributes:
                        model.InStock = (await _productAttributeService
                            .GetAllProductAttributeCombinationsAsync(product.Id))
                            ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                            ?? false;
                        break;
                }
            }

            //breadcrumb
            //do not prepare this model for the associated products. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
            {
                model.Breadcrumb = await PrepareProductBreadcrumbModelAsync(product);
            }

            //product tags
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductTags = await PrepareProductTagModelsAsync(product);
            }

            //pictures and videos
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            IList<PictureModel> allPictureModels;
            IList<VideoModel> allVideoModels;
            (model.DefaultPictureModel, allPictureModels, allVideoModels) = await PrepareProductDetailsPictureModelAsync(product, isAssociatedProduct);
            model.PictureModels = allPictureModels;
            model.VideoModels = allVideoModels;

            //price
            if (mainProduct != null)
                model.ProductPrice = await PrepareProductPriceModelAsync(mainProduct);
            else
                model.ProductPrice = await PrepareProductPriceModelAsync(product);

            //'Add to cart' model
            model.AddToCart = await PrepareProductAddToCartModelAsync(product, mainProduct, updatecartitem);
            var customer = await _workContext.GetCurrentCustomerAsync();
            //gift card
            if (product.IsGiftCard)
            {
                model.GiftCard.IsGiftCard = true;
                model.GiftCard.GiftCardType = product.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = await _customerService.GetCustomerFullNameAsync(customer);
                    model.GiftCard.SenderEmail = customer.Email;
                }
                else
                {
                    _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out var giftCardRecipientName, out var giftCardRecipientEmail,
                        out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            //product attributes
            model.ProductAttributes = await PrepareProductAttributeModelsAsync(product, updatecartitem);

            //product specifications
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
            }

            //product review overview
            model.ProductReviewOverview = await PrepareProductReviewOverviewModelAsync(product);

            //tier prices
            if (product.HasTierPrices && await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = await PrepareProductTierPriceModelsAsync(product);
            }

            //manufacturers
            model.ProductManufacturers = await PrepareProductManufacturerModelsAsync(product);

            //rental products
            if (product.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            //estimate shipping
            if (_shippingSettings.EstimateShippingProductPageEnabled && !model.IsFreeShipping)
            {
                var wrappedProduct = new ShoppingCartItem
                {
                    StoreId = store.Id,
                    ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                    CustomerId = customer.Id,
                    ProductId = product.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };

                var estimateShippingModel = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(new[] { wrappedProduct });

                model.ProductEstimateShipping.ProductId = product.Id;
                model.ProductEstimateShipping.RequestDelay = estimateShippingModel.RequestDelay;
                model.ProductEstimateShipping.Enabled = estimateShippingModel.Enabled;
                model.ProductEstimateShipping.CountryId = estimateShippingModel.CountryId;
                model.ProductEstimateShipping.StateProvinceId = estimateShippingModel.StateProvinceId;
                model.ProductEstimateShipping.ZipPostalCode = estimateShippingModel.ZipPostalCode;
                model.ProductEstimateShipping.UseCity = estimateShippingModel.UseCity;
                model.ProductEstimateShipping.City = estimateShippingModel.City;
                model.ProductEstimateShipping.AvailableCountries = estimateShippingModel.AvailableCountries;
                model.ProductEstimateShipping.AvailableStates = estimateShippingModel.AvailableStates;
            }

            //associated products
            if (product.ProductType == ProductType.GroupedProduct)
            {
                //ensure no circular references
                if (!isAssociatedProduct)
                {
                    var associatedProducts = await _productService.GetAssociatedProductsAsync(product.Id, store.Id);
                    foreach (var associatedProduct in associatedProducts)
                    {
                        var productDetails = await PrepareProductDetailsModelAsync(associatedProduct, null, true);

                        if (productDetails != null)
                            model.AssociatedProducts.Add(productDetails);
                    }
                }
                model.InStock = model.AssociatedProducts.Any(associatedProduct => associatedProduct.InStock);
            }
            var extendedReModel = new ZarIranProductDetailsModel();

            if (model.AssociatedProducts.Any())
            {
                var bundleProductsIncludeProductsCategories = await _bundleProductService.GetBundleProductsInludeProductsByProductIdAsync(model.AssociatedProducts.FirstOrDefault().Id);

                extendedReModel.BundleProducts = await bundleProductsIncludeProductsCategories.BundleProducts.SelectAwait(async bundleProduct =>
                {
                    var currentProduct = bundleProductsIncludeProductsCategories.Products.FirstOrDefault(f => f.Id == bundleProduct.ProductId2);

                    if (currentProduct is null)
                        throw new ArgumentNullException(nameof(currentProduct));

                    var parentCurrentProduct = await _productService.GetProductByIdAsync(currentProduct.ParentGroupedProductId);

                    if (parentCurrentProduct is null)
                        throw new ArgumentNullException(nameof(parentCurrentProduct));

                    var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(currentProduct, (await _priceCalculationService.GetFinalPriceAsync(currentProduct, customer, store, includeDiscounts: false)).finalPrice);
                    var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(currentProduct, (await _priceCalculationService.GetFinalPriceAsync(currentProduct, customer, store)).finalPrice);

                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

                    var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
                    var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

                    var picutres = await PrepareProductDetailsPictureModelAsync(parentCurrentProduct, false);
                    var productManufacturer = model.ProductManufacturers.FirstOrDefault();
                    var manufacturer = productManufacturer != null ? await _manufacturerService.GetManufacturerByIdAsync(productManufacturer.Id) : null;

                    return new BundleProductModel
                    {
                        Count = bundleProduct.Count,
                        Picture = picutres.pictureModel,
                        ManufacturerName = model.ProductManufacturers.FirstOrDefault()?.Name,
                        PriceStr = await _priceFormatter.FormatPriceAsync(bundleProduct.Price),
                        OldPriceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithoutDiscount),
                        DiscountPercent = bundleProduct.Price > 0 ? (int)(((finalPriceWithoutDiscount - bundleProduct.Price) / finalPriceWithoutDiscount) * 100) : 0,
                        SeName = await _urlRecordService.GetSeNameAsync(parentCurrentProduct),
                        Name = parentCurrentProduct.Name,
                        ManufacturerPicture = await PrepareManufacturerPictureAsync(manufacturer),
                        ManufacturerSeName = await _urlRecordService.GetSeNameAsync(manufacturer)
                    };
                }).ToListAsync();

                extendedReModel.BDPCategoriesName = string.Join(" - ", bundleProductsIncludeProductsCategories.categories.Where(w => w.Id != _coreSettings.AllProductsCategoryId).DistinctBy(cat => cat.Id).Select(s => s.Name));

            }

            model.CopyPropertiesTo(extendedReModel);

            //token formatting
            await PrepareTokenFormatting(extendedReModel, isAssociatedProduct);



            extendedReModel.AdminComment = product.AdminComment;
            return extendedReModel;
        }

        #endregion

        #region Utilities

        protected async Task<ProductDetailsModel.AddToCartModel> PrepareProductAddToCartModelAsync(Product product, Product mainProduct, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductDetailsModel.AddToCartModel
            {
                ProductId = product.Id
            };

            product = mainProduct ?? product;

            if (updatecartitem != null)
            {
                model.UpdatedShoppingCartItemId = updatecartitem.Id;
                model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
            }

            //quantity
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity;
            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                model.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }
            //minimum quantity notification
            if (product.OrderMinimumQuantity > 1)
            {
                model.MinimumQuantityNotification = string.Format(await _localizationService.GetResourceAsync("Products.MinimumQuantityNotification"), product.OrderMinimumQuantity);
            }

            //'add to cart', 'add to wishlist' buttons
            model.DisableBuyButton = product.DisableBuyButton || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);
            model.DisableWishlistButton = product.DisableWishlistButton || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist);
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.DisableBuyButton = true;
                model.DisableWishlistButton = true;
            }
            //pre-order
            if (product.AvailableForPreOrder)
            {
                model.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;

                if (model.AvailableForPreOrder &&
                    model.PreOrderAvailabilityStartDateTimeUtc.HasValue &&
                    _catalogSettings.DisplayDatePreOrderAvailability)
                {
                    model.PreOrderAvailabilityStartDateTimeUserTime =
                        (await _dateTimeHelper.ConvertToUserTimeAsync(model.PreOrderAvailabilityStartDateTimeUtc.Value)).ToString("D");
                }
            }
            //rental
            model.IsRental = product.IsRental;

            //customer entered price
            model.CustomerEntersPrice = product.CustomerEntersPrice;
            if (!model.CustomerEntersPrice)
                return model;

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var minimumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MinimumCustomerEnteredPrice, currentCurrency);
            var maximumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MaximumCustomerEnteredPrice, currentCurrency);

            model.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
            model.CustomerEnteredPriceRange = string.Format(await _localizationService.GetResourceAsync("Products.EnterProductPrice.Range"),
                await _priceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
                await _priceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false));

            return model;
        }

        protected override async Task<ProductDetailsModel.ProductPriceModel> PrepareProductPriceModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new ProductDetailsModel.ProductPriceModel
            {
                ProductId = product.Id
            };

            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.HidePrices = false;
                if (product.CustomerEntersPrice)
                {
                    model.CustomerEntersPrice = true;
                }
                else
                {
                    if (product.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
                    {
                        model.CallForPrice = true;
                    }
                    else
                    {
                        var customer = await _workContext.GetCurrentCustomerAsync();
                        var store = await _storeContext.GetCurrentStoreAsync();
                        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

                        var (oldPriceBase, _) = await _taxService.GetProductPriceAsync(product, product.OldPrice);

                        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product, customer, store, includeDiscounts: false)).finalPrice);
                        var (finalPriceWithDiscountBase, _) = await _taxService.GetProductPriceAsync(product, (await _priceCalculationService.GetFinalPriceAsync(product, customer, store)).finalPrice);

                        var (calculatedOldPrice, calculatedPrice) = await _zarIranPriceCalculationService.GetCalculatedProductPriceAsync(product.Id);

                        var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(
                            calculatedOldPrice, currentCurrency);

                        var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(
                            calculatedPrice, currentCurrency);

                        var finalPriceWithDiscount = finalPriceWithoutDiscount;

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        {
                            model.OldPrice = await _priceFormatter.FormatPriceAsync(oldPrice);
                            model.OldPriceValue = oldPrice;
                        }

                        model.Price = await _priceFormatter.FormatPriceAsync(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        {
                            model.PriceWithDiscount = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.PriceWithDiscountValue = finalPriceWithDiscount;
                        }

                        model.PriceValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                            && product.IsShipEnabled &&
                            !product.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(product, finalPriceWithDiscountBase);
                        model.BasePricePAngVValue = finalPriceWithDiscountBase;
                        //currency code
                        model.CurrencyCode = currentCurrency.CurrencyCode;

                        //rental
                        if (product.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.RentalPrice = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceStr);
                            model.RentalPriceValue = finalPriceWithDiscount;
                        }
                    }
                }
            }
            else
            {
                model.HidePrices = true;
                model.OldPrice = null;
                model.OldPriceValue = null;
                model.Price = null;
            }

            return model;
        }




        private async Task<PictureModel> PrepareManufacturerPictureAsync(Manufacturer manufacturer)
        {
            var pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var manufacturerPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturerPictureModelKey,
                manufacturer, pictureSize, true, await _workContext.GetWorkingLanguageAsync(),
                _webHelper.IsCurrentConnectionSecured(), currentStore);

            return await _staticCacheManager.GetAsync(manufacturerPictureCacheKey, async () =>
            {
                var picture = await _pictureService.GetPictureByIdAsync(manufacturer?.PictureId ?? 0);
                string fullSizeImageUrl, imageUrl;

                (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = fullSizeImageUrl,
                    ImageUrl = imageUrl,
                    Title = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageLinkTitleFormat"), manufacturer?.Name),
                    AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageAlternateTextFormat"), manufacturer?.Name)
                };

                return pictureModel;
            });
        }

        public override async Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IEnumerable<Product> products, bool preparePriceModel = true, bool preparePictureModel = true, int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false, bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
                throw new ArgumentNullException(nameof(products));

            var models = new List<ProductOverviewModel>();

            var bundleProducts = (await _bundleProductService.GetBundleProductsAsync(products.Select(s => s.Id).ToList())).ToHashSet();

            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var currentCustomerRoleIds = await _customerService.GetCustomerRoleIdsAsync(currentCustomer);

            var customerRoleIdsIncludeDiscount = await GetCustomerRoleIdsIncludeTradeDiscountAsync();

            var hasTradeDiscount = currentCustomerRoleIds.Intersect(customerRoleIdsIncludeDiscount).Any();

            foreach (var product in products)
            {
                var model = new ZarIranProductOverviewModel
                {
                    Id = product.Id,
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                    FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                    SeName = await _urlRecordService.GetSeNameAsync(product),
                    Sku = product.Sku,
                    ProductType = product.ProductType,
                    Weight = product.Weight,
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow),
                    BundleItemsCount = bundleProducts.Count(a => a.ProductId1 == product.Id),
                    HasTradeDiscount = hasTradeDiscount
                };

                //price
                if (preparePriceModel)
                {
                    model.ProductPrice = await PrepareProductOverviewPriceModelAsync(product, forceRedirectionAfterAddingToCart);
                }

                //picture
                if (preparePictureModel)
                {
                    model.PictureModels = await PrepareProductOverviewPicturesModelAsync(product, productThumbPictureSize);
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
                }

                //reviews
                model.ReviewOverviewModel = await PrepareProductReviewOverviewModelAsync(product);

                models.Add(model);
            }

            return models;
        }

        private async Task<List<int>> GetCustomerRoleIdsIncludeTradeDiscountAsync()
        {
            var discountRequirements = await _discountService.GetAllDiscountRequirementsAsync(_coreSettings.TradeDiscountId);
            List<int> customerRoleIdsIncludeTradeDisocunt = new();
            var format = "DiscountRequirement.MustBeAssignedToCustomerRole-{0}";

            foreach (var discountRequirement in discountRequirements)
            {
                var customerRoleId = await _settingService.GetSettingByKeyAsync<int>(string.Format(format, discountRequirement.Id));

                customerRoleIdsIncludeTradeDisocunt.Add(customerRoleId);
            }

            return customerRoleIdsIncludeTradeDisocunt;
        }


        private async Task PrepareTokenFormatting(ZarIranProductDetailsModel model, bool isAssocaitaed)
        {
            var tokens = await BuildToken(model);

            model.FullDescription = TokenFormatting(model.FullDescription, tokens);
            model.ShortDescription = TokenFormatting(model.ShortDescription, tokens);

            if (!isAssocaitaed)
            {
                model.MetaDescription = TokenFormatting(model.MetaDescription, tokens);
                model.MetaKeywords = TokenFormatting(model.MetaKeywords, tokens);
                model.MetaTitle = TokenFormatting(model.MetaTitle, tokens);
            }
        }

        private string TokenFormatting(string property, Dictionary<string, string> tokens)
        {
            if (string.IsNullOrEmpty(property)) return null;

            foreach (var token in tokens)
            {
                property = property.Replace(token.Key, token.Value, StringComparison.OrdinalIgnoreCase);
            }

            return property;
        }

        private async Task<Dictionary<string, string>> BuildToken(ZarIranProductDetailsModel model)
        {
            var token = new Dictionary<string, string>
            {
                ["[ProductName]"] = model.Name,
                ["[Price]"] = model.AssociatedProducts.FirstOrDefault()?.ProductPrice.Price,
                ["[ManufacturerPartNumber]"] = model.ManufacturerPartNumber,
            };


            var resourceToken = (await _localizationService.GetAllResourceValuesAsync((await _workContext.GetWorkingLanguageAsync()).Id, true))
                                .Where(a => a.Key.StartsWith("[RSC:", StringComparison.OrdinalIgnoreCase));

            foreach (var resource in resourceToken)
                token.Add(resource.Key, resource.Value.Value);


            return token;
        }
        #endregion

    }
}