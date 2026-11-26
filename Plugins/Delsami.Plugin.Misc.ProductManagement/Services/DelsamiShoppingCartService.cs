using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Irony.Parsing;
using MailKit;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public class DelsamiShoppingCartService : ShoppingCartService, IShoppingCartService
    {
        #region Fields

        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly OrderSettings _orderSettings;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDateRangeService _dateRangeService;
        private readonly IReservedStockQuantityService _reservedStockQuantityService;
        private readonly ICustomDelsamiProductService _customDelsamiProductService;


        #endregion

        #region Ctor

        public DelsamiShoppingCartService(CatalogSettings catalogSettings,
                                           IAclService aclService,
                                           IActionContextAccessor actionContextAccessor,
                                           ICheckoutAttributeParser checkoutAttributeParser,
                                           ICheckoutAttributeService checkoutAttributeService,
                                           ICurrencyService currencyService,
                                           ICustomerService customerService,
                                           IDateRangeService dateRangeService,
                                           IDateTimeHelper dateTimeHelper,
                                           IGenericAttributeService genericAttributeService,
                                           ILocalizationService localizationService,
                                           IPermissionService permissionService,
                                           IPriceCalculationService priceCalculationService,
                                           IPriceFormatter priceFormatter,
                                           IProductAttributeParser productAttributeParser,
                                           IProductAttributeService productAttributeService,
                                           IProductService productService,
                                           IRepository<ShoppingCartItem> sciRepository,
                                           IShippingService shippingService,
                                           IStaticCacheManager staticCacheManager,
                                           IStoreContext storeContext,
                                           IStoreService storeService,
                                           IStoreMappingService storeMappingService,
                                           IUrlHelperFactory urlHelperFactory,
                                           IUrlRecordService urlRecordService,
                                           IWorkContext workContext,
                                           OrderSettings orderSettings,
                                           ShoppingCartSettings shoppingCartSettings,
                                           IVirtualProductMappingService virtualProductMappingService
,
                                           IReservedStockQuantityService reservedStockQuantityService,
                                           ICustomDelsamiProductService customDelsamiProductService) : base(catalogSettings,
                                                                                                              aclService,
                                                                                                              actionContextAccessor,
                                                                                                              checkoutAttributeParser,
                                                                                                              checkoutAttributeService,
                                                                                                              currencyService,
                                                                                                              customerService,
                                                                                                              dateRangeService,
                                                                                                              dateTimeHelper,
                                                                                                              genericAttributeService,
                                                                                                              localizationService,
                                                                                                              permissionService,
                                                                                                              priceCalculationService,
                                                                                                              priceFormatter,
                                                                                                              productAttributeParser,
                                                                                                              productAttributeService,
                                                                                                              productService,
                                                                                                              sciRepository,
                                                                                                              shippingService,
                                                                                                              staticCacheManager,
                                                                                                              storeContext,
                                                                                                              storeService,
                                                                                                              storeMappingService,
                                                                                                              urlHelperFactory,
                                                                                                              urlRecordService,
                                                                                                              workContext,
                                                                                                              orderSettings,
                                                                                                              shoppingCartSettings)
        {
            _virtualProductMappingService = virtualProductMappingService;
            _localizationService = localizationService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _orderSettings = orderSettings;
            _workContext = workContext;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _shoppingCartSettings = shoppingCartSettings;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _dateRangeService = dateRangeService;
            _reservedStockQuantityService = reservedStockQuantityService;
            _customDelsamiProductService = customDelsamiProductService;
        }

        #endregion

        #region Methods

        protected override async Task<IList<string>> GetStandardWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product, string attributesXml, decimal customerEnteredPrice, int quantity, int shoppingCartItemId, int storeId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var warnings = new List<string>();


            if (product.Deleted)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductDeleted"));
                return warnings;
            }

            Product mainProduct = null;

            //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
            //DeliveryDateId is 'MainProductId'
            if (product.AllowAddingOnlyExistingAttributeCombinations)
                mainProduct = await _productService.GetProductByIdAsync(product.DeliveryDateId);
            else
                mainProduct = product;
            //deleted

            if (mainProduct is null)
                throw new NullReferenceException(nameof(mainProduct));

            if (mainProduct.Deleted)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductDeleted"));
                return warnings;
            }

            //published
            if (!mainProduct.Published)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
            }

            if (mainProduct.ProductType != ProductType.SimpleProduct)
            {
                warnings.Add("This is not simple product");
            }

            if (!await _aclService.AuthorizeAsync(mainProduct, customer))
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
            }

            if (!await _storeMappingService.AuthorizeAsync(mainProduct, storeId))
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
            }

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && mainProduct.DisableBuyButton)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.BuyingDisabled"));
            }


            if (shoppingCartType == ShoppingCartType.ShoppingCart && mainProduct.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                warnings.Add(await _localizationService.GetResourceAsync("Products.CallForPrice"));
            }

            //customer entered price
            if (mainProduct.CustomerEntersPrice)
            {
                if (customerEnteredPrice < mainProduct.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > mainProduct.MaximumCustomerEnteredPrice)
                {
                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                    var minimumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(mainProduct.MinimumCustomerEnteredPrice, currentCurrency);
                    var maximumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(mainProduct.MaximumCustomerEnteredPrice, currentCurrency);
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.CustomerEnteredPrice.RangeError"),
                        await _priceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
                        await _priceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false)));
                }
            }

            //quantity validation

            var hasQtyWarningsMainProduct = false;
            if (quantity < mainProduct.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MinimumQuantity"), mainProduct.OrderMinimumQuantity));
                hasQtyWarningsMainProduct = true;
            }

            if (quantity > mainProduct.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumQuantity"), mainProduct.OrderMaximumQuantity));
                hasQtyWarningsMainProduct = true;
            }

            var allowedQuantitiesMainProduct = _productService.ParseAllowedQuantities(mainProduct);
            if (allowedQuantitiesMainProduct.Length > 0 && !allowedQuantitiesMainProduct.Contains(quantity))
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantitiesMainProduct)));
            }

            var validateOutOfStockMainProduct = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
            if (validateOutOfStockMainProduct && !hasQtyWarningsMainProduct)
            {
                switch (mainProduct.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        //do nothing
                        break;
                    case ManageInventoryMethod.ManageStock:
                        if (mainProduct.BackorderMode == BackorderMode.NoBackorders)
                        {

                            var maximumQuantityCanBeAdded = await _productService.GetTotalStockQuantityAsync(mainProduct) - (await _reservedStockQuantityService.GetReservedStockQuantityCountAsync(mainProduct.Id));

                            warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, maximumQuantityCanBeAdded));

                            if (warnings.Any())
                                return warnings;

                            //validate product quantity with non combinable product attributes
                            var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(mainProduct.Id);
                            if (productAttributeMappings?.Any() == true)
                            {
                                var onlyCombinableAttributes = productAttributeMappings.All(mapping => !mapping.IsNonCombinable());
                                if (!onlyCombinableAttributes)
                                {
                                    var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
                                    var totalAddedQuantity = cart
                                        .Where(item => item.ProductId == mainProduct.Id && item.Id != shoppingCartItemId)
                                        .Sum(product => product.Quantity);

                                    totalAddedQuantity += quantity;

                                    //counting a product into bundles
                                    foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                    {
                                        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
                                        foreach (var attributeValue in attributeValues)
                                        {
                                            if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == mainProduct.Id)
                                                totalAddedQuantity += bundle.Quantity * attributeValue.Quantity;
                                        }
                                    }

                                    warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalAddedQuantity, maximumQuantityCanBeAdded));
                                }
                            }

                            if (warnings.Any())
                                return warnings;

                            //validate product quantity and product quantity into bundles
                            if (string.IsNullOrEmpty(attributesXml))
                            {
                                var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
                                var totalQuantityInCart = cart.Where(item => item.ProductId == product.Id && item.Id != shoppingCartItemId && string.IsNullOrEmpty(item.AttributesXml))
                                    .Sum(product => product.Quantity);

                                totalQuantityInCart += quantity;

                                foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                {
                                    var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == product.Id)
                                            totalQuantityInCart += bundle.Quantity * attributeValue.Quantity;
                                    }
                                }

                                warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalQuantityInCart, maximumQuantityCanBeAdded));
                            }
                        }

                        break;
                    case ManageInventoryMethod.ManageStockByAttributes:
                        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                        if (combination != null)
                        {
                            //combination exists
                            //let's check stock level
                            if (!combination.AllowOutOfStockOrders)
                                warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, combination.StockQuantity));
                        }
                        else
                        {
                            //combination doesn't exist
                            if (product.AllowAddingOnlyExistingAttributeCombinations)
                            {
                                //maybe, is it better  to display something like "No such product/combination" message?
                                var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                                var warning = productAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                                    : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                                        await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                                warnings.Add(warning);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //availability dates
            var availableStartDateErrorMainProduct = false;
            if (product.AvailableStartDateTimeUtc.HasValue)
            {
                var availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(DateTime.UtcNow) > 0)
                {
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
                    availableStartDateErrorMainProduct = true;
                }
            }

            if (!product.AvailableEndDateTimeUtc.HasValue || availableStartDateErrorMainProduct)
                return warnings;

            var availableEndDateTimeMainProduct = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
            if (availableEndDateTimeMainProduct.CompareTo(DateTime.UtcNow) < 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
            }

            return warnings;
        }

        public override async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(Product product,
                                                                                                                             Customer customer,
                                                                                                                             Store store,
                                                                                                                             ShoppingCartType shoppingCartType,
                                                                                                                             int quantity,
                                                                                                                             string attributesXml,
                                                                                                                             decimal customerEnteredPrice,
                                                                                                                             DateTime? rentalStartDate,
                                                                                                                             DateTime? rentalEndDate,
                                                                                                                             bool includeDiscounts)
        {
            //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
            //DeliveryDateId is 'MainProductId'
            if (product.AllowAddingOnlyExistingAttributeCombinations && product.DeliveryDateId > 0)
                product = await _productService.GetProductByIdAsync(product.DeliveryDateId);

            return await base.GetUnitPriceAsync(product, customer, store, shoppingCartType, quantity, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, includeDiscounts);
        }


        protected override async Task<IList<string>> GetQuantityProductWarningsAsync(Product product, int quantity, int maximumQuantityCanBeAdded)
        {
            List<string> warnings = new();

            var isVirtual = product.AllowAddingOnlyExistingAttributeCombinations;

            //AllowAddingOnlyExistingAttributeCombinations is 'IsVirtual'
            //DeliveryDateId is 'MainProductId'

            Product mainProduct = null;

            if (isVirtual)
                mainProduct = await _productService.GetProductByIdAsync(product.DeliveryDateId);
            else
                mainProduct = product;

            if (mainProduct is null)
                throw new NullReferenceException(nameof(mainProduct));

            var virtualProducts = await _customDelsamiProductService.GetVirtualProductsByMainProductIdAsync(mainProduct.Id);
            var virtualProductIds = virtualProducts.Select(vp => vp.Id).ToList();
            virtualProductIds.Add(mainProduct.Id);

            var customer = await _workContext.GetCurrentCustomerAsync();
            var cartItems = await base.GetShoppingCartAsync(customer);

            //var matchingItem = cartItems.FirstOrDefault(ci => ci.ProductId == product.Id);
            //var shoppingCartItemId = matchingItem?.Id ?? 0;

            var existingRelatedProductsQty = cartItems
                .Where(ci => virtualProductIds.Contains(ci.ProductId) && ci.ProductId != product.Id)
                .Sum(ci => ci.Quantity);

            //var currentProductInCartItems = cartItems.FirstOrDefault(f => f.ProductId == mainProduct.Id);

            //if (currentProductInCartItems is not null)
            //    quantity += currentProductInCartItems.Quantity;

            var totalRelatedProductsRequestedQty = existingRelatedProductsQty + quantity;

            if (maximumQuantityCanBeAdded < totalRelatedProductsRequestedQty)
            {
                if (maximumQuantityCanBeAdded <= 0)
                {
                    var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                    var warning = productAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                        : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                            await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                    warnings.Add(warning);
                }
                else
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
            }
            //}

            return warnings;
        }

        #endregion
    }
}