using ZarIran.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Requests;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Factories
{
    public class FulfillmentDecisionFactory : IFulfillmentDecisionFactory
    {
        #region Fields

        private readonly IStateProvinceService _stateProvinceService;
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FulfillmentDecisionFactory(IStateProvinceService stateProvinceService,
                                          IProductService productService,
                                          ILogger logger)
        {
            _stateProvinceService = stateProvinceService;
            _productService = productService;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<FulfillmentDecisionRequestDto> PrepareFulfillmentDecisionRequestAsync(IList<ShoppingCartItem> cartItems, Address shippingAddress)
        {
            try
            {
                if (!shippingAddress.StateProvinceId.HasValue)
                    return null;

                if (string.IsNullOrWhiteSpace(shippingAddress.City))
                    return null;

                var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(shippingAddress.StateProvinceId.Value);

                FulfillmentDecisionRequestDto request = new();

                request.ShippingCity = shippingAddress.City;
                request.ShippingProvince = stateProvince.Name;

                foreach (var cartItem in cartItems)
                {
                    Product product = null; ;

                    product = await _productService.GetProductByIdAsync(cartItem.ProductId);

                    request.Items.Add(new FulfillmentDecisionItemDto
                    {
                        Sku = product.Sku,
                        RequestedQuantity = cartItem.Quantity
                    });
                }

                return request;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return null;
        }

        #endregion
    }
}
