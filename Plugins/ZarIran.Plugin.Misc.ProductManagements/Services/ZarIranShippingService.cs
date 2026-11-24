using ZarIran.Plugin.Misc.Core.Services;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class ZarIranShippingService
        : IShippingService
    {
        #region Fields
        private readonly IShippingService _inner;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IVirtualProductMappingService _virtualProductMappingService;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;

        #endregion

        #region Ctor
        public ZarIranShippingService(IShippingService inner,
                                       IProductService productService,
                                       IProductAttributeParser productAttributeParser,
                                       IVirtualProductMappingService virtualProductMappingService,
                                       IStoreContext storeContext,
                                       IGenericAttributeService genericAttributeService,
                                       ICheckoutAttributeParser checkoutAttributeParser)
        {
            _inner = inner;
            _productService = productService;
            _productAttributeParser = productAttributeParser;
            _virtualProductMappingService = virtualProductMappingService;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _checkoutAttributeParser = checkoutAttributeParser;
        }
        #endregion

        #region Methods
        public async Task<bool> CountryRestrictionExistsAsync(ShippingMethod shippingMethod, int countryId)
        {
            return await _inner.CountryRestrictionExistsAsync(shippingMethod, countryId);
        }

        public async Task<(IList<GetShippingOptionRequest> shipmentPackages, bool shippingFromMultipleLocations)> CreateShippingOptionRequestsAsync(IList<ShoppingCartItem> cart, Address shippingAddress, int storeId)
        {
            return await _inner.CreateShippingOptionRequestsAsync(cart, shippingAddress, storeId);
        }

        public async Task DeleteShippingMethodAsync(ShippingMethod shippingMethod)
        {
            await _inner.DeleteShippingMethodAsync(shippingMethod);
        }

        public async Task DeleteShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
        {
            await _inner.DeleteShippingMethodCountryMappingAsync(shippingMethodCountryMapping);
        }

        public async Task DeleteWarehouseAsync(Warehouse warehouse)
        {
            await _inner.DeleteWarehouseAsync(warehouse);
        }

        public async Task<decimal> GetAdditionalShippingChargeAsync(ShoppingCartItem shoppingCartItem)
        {
            if (await IsFreeShippingAsync(shoppingCartItem))
                return decimal.Zero;

            decimal additionalShippingCharge = decimal.Zero;

            var (isVirtualProduct, mainProduct) =
                await _virtualProductMappingService.GetMainProductByVirtualProductIdAsync(shoppingCartItem.ProductId);

            if (isVirtualProduct)
            {
                //var product = await _productService.GetProductByIdAsync(mainProduct.Id);
                additionalShippingCharge = (mainProduct?.AdditionalShippingCharge ?? decimal.Zero) * shoppingCartItem.Quantity;
            }
            else
            {
                var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
                additionalShippingCharge = (product?.AdditionalShippingCharge ?? decimal.Zero) * shoppingCartItem.Quantity;
            }

            //get additional shipping charge of the product attributes
            if (!string.IsNullOrEmpty(shoppingCartItem.AttributesXml))
            {
                additionalShippingCharge += await (await _productAttributeParser.ParseProductAttributeValuesAsync(shoppingCartItem.AttributesXml))
                    .Where(attributeValue => attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    .SumAwaitAsync(async attributeValue =>
                        (await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId))?.AdditionalShippingCharge ?? decimal.Zero);
            }

            return additionalShippingCharge;
        }

        public async Task<IList<ShippingMethod>> GetAllShippingMethodsAsync(int? filterByCountryId = null)
        {
            return await _inner.GetAllShippingMethodsAsync(filterByCountryId);
        }

        public async Task<IList<Warehouse>> GetAllWarehousesAsync(string name = null)
        {
            return await _inner.GetAllWarehousesAsync(name);
        }

        public async Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> packageItems, bool ignoreFreeShippedItems = false)
        {
            return await _inner.GetDimensionsAsync(packageItems, ignoreFreeShippedItems);
        }

        public async Task<Warehouse> GetNearestWarehouseAsync(Address address, IList<Warehouse> warehouses = null)
        {
            return await _inner.GetNearestWarehouseAsync(address, warehouses);
        }

        public async Task<GetPickupPointsResponse> GetPickupPointsAsync(IList<ShoppingCartItem> cart, Address address, Customer customer = null, string providerSystemName = null, int storeId = 0)
        {
            return await _inner.GetPickupPointsAsync(cart, address, customer, providerSystemName, storeId);
        }

        public async Task<ShippingMethod> GetShippingMethodByIdAsync(int shippingMethodId)
        {
            return await _inner.GetShippingMethodByIdAsync(shippingMethodId);
        }

        public async Task<IList<ShippingMethodCountryMapping>> GetShippingMethodCountryMappingAsync(int shippingMethodId, int countryId)
        {
            return await _inner.GetShippingMethodCountryMappingAsync(shippingMethodId, countryId);
        }

        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart, Address shippingAddress, Customer customer = null, string allowedShippingRateComputationMethodSystemName = "", int storeId = 0)
        {
            return await _inner.GetShippingOptionsAsync(cart, shippingAddress, customer, allowedShippingRateComputationMethodSystemName, storeId);
        }

        public async Task<decimal> GetShoppingCartItemWeightAsync(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);

            return await GetShoppingCartItemWeightAsync(product, shoppingCartItem.AttributesXml, ignoreFreeShippedItems);
        }

        public async Task<decimal> GetShoppingCartItemWeightAsync(Product product, string attributesXml, bool ignoreFreeShippedItems = false)
        {
            var virtualProductResult = await _virtualProductMappingService.GetMainProductByVirtualProductIdAsync(product.Id);

            if (virtualProductResult.IsVirtualPrdocut)
                product = virtualProductResult.MainProduct;

            var mainProduct = virtualProductResult.IsVirtualPrdocut ? virtualProductResult.MainProduct : product;

            return await _inner.GetShoppingCartItemWeightAsync(mainProduct, attributesXml, ignoreFreeShippedItems);
        }

        public async Task<decimal> GetTotalWeightAsync(GetShippingOptionRequest request, bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var totalWeight = decimal.Zero;

            //shopping cart items
            foreach (var packageItem in request.Items)
                totalWeight += await GetShoppingCartItemWeightAsync(packageItem.ShoppingCartItem, ignoreFreeShippedItems) * packageItem.GetQuantity();

            //checkout attributes
            if (request.Customer is null || !includeCheckoutAttributes)
                return totalWeight;
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(request.Customer, NopCustomerDefaults.CheckoutAttributes, store.Id);
            if (string.IsNullOrEmpty(checkoutAttributesXml))
                return totalWeight;
            var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributesXml);
            foreach (var attributeValue in await attributeValues.SelectMany(x => x.values).ToListAsync())
                totalWeight += attributeValue.WeightAdjustment;

            return totalWeight;
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int warehouseId)
        {
            return await _inner.GetWarehouseByIdAsync(warehouseId);
        }

        public async Task InsertShippingMethodAsync(ShippingMethod shippingMethod)
        {
            await _inner.InsertShippingMethodAsync(shippingMethod);
        }

        public async Task InsertShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping)
        {
            await _inner.InsertShippingMethodCountryMappingAsync(shippingMethodCountryMapping);
        }

        public async Task InsertWarehouseAsync(Warehouse warehouse)
        {
            await _inner.InsertWarehouseAsync(warehouse);
        }

        public async Task<bool> IsFreeShippingAsync(ShoppingCartItem shoppingCartItem)
        {
            return await _inner.IsFreeShippingAsync(shoppingCartItem);
        }

        public async Task<bool> IsShipEnabledAsync(ShoppingCartItem shoppingCartItem)
        {
            return await _inner.IsShipEnabledAsync(shoppingCartItem);
        }

        public async Task UpdateShippingMethodAsync(ShippingMethod shippingMethod)
        {
            await _inner.UpdateShippingMethodAsync(shippingMethod);
        }

        public async Task UpdateWarehouseAsync(Warehouse warehouse)
        {
            await _inner.UpdateWarehouseAsync(warehouse);
        }
        #endregion

    }
}
