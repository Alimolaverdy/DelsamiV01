using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Payments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Services
{
    public class ZarIranPaymentService : PaymentService, IZarIranPaymentService
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPaymentMethodDiscountService _paymentMethodDiscountService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public ZarIranPaymentService(ICustomerService customerService,
                                      IHttpContextAccessor httpContextAccessor,
                                      IPaymentPluginManager paymentPluginManager,
                                      IPriceCalculationService priceCalculationService,
                                      PaymentSettings paymentSettings,
                                      ShoppingCartSettings shoppingCartSettings,
                                      IPaymentMethodDiscountService paymentMethodDiscountService,
                                      IProductService productService) : base(customerService,
                                                                                        httpContextAccessor,
                                                                                        paymentPluginManager,
                                                                                        priceCalculationService,
                                                                                        paymentSettings,
                                                                                        shoppingCartSettings)
        {
            _customerService = customerService;
            _paymentPluginManager = paymentPluginManager;
            _shoppingCartSettings = shoppingCartSettings;
            _priceCalculationService = priceCalculationService;
            _paymentMethodDiscountService = paymentMethodDiscountService;
            _productService = productService;
        }

        #endregion

        #region Methods

        //public override async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart, string paymentMethodSystemName)
        //{
        //    if (string.IsNullOrEmpty(paymentMethodSystemName))
        //        return decimal.Zero;

        //    var customer = await _customerService.GetCustomerByIdAsync(cart.FirstOrDefault()?.CustomerId ?? 0);
        //    var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, cart.FirstOrDefault()?.StoreId ?? 0);
        //    if (paymentMethod == null)
        //        return decimal.Zero;

        //    var result = await paymentMethod.GetAdditionalHandlingFeeAsync(cart);
        //    if (result < decimal.Zero)
        //        result = decimal.Zero;

        //    //var paymentMethodDiscount = await _paymentMethodDiscountService.GetActiveAsync(paymentMethodSystemName);

        //    //if (paymentMethodDiscount != null)
        //    //{
        //    //    var products = await _productService.GetProductsByIdsAsync(cart.Select(s => s.ProductId).ToArray());
        //    //    decimal productsPrice = 0;

        //    //    foreach (var product in products)
        //    //        productsPrice += product.Price * (cart.FirstOrDefault(f => f.ProductId == product.Id).Quantity);

        //    //    var paymentMethodDiscountValue = productsPrice * (decimal)((decimal)paymentMethodDiscount.DiscountPercent / 100);
        //    //    result = -paymentMethodDiscountValue;
        //    //}

        //    result = await CalculatePaymentMethodDiscountAsync(cart, paymentMethodSystemName);

        //    if (!_shoppingCartSettings.RoundPricesDuringCalculation)
        //        return result;

        //    result = await _priceCalculationService.RoundPriceAsync(result);

        //    return result;
        //}

        public async Task<decimal> CalculatePaymentMethodDiscountAsync(IList<ShoppingCartItem> cart, string paymentMethodSystemName)
        {
            decimal result = 0;

            var paymentMethodDiscount = await _paymentMethodDiscountService.GetActiveAsync(paymentMethodSystemName);

            if (paymentMethodDiscount != null)
            {
                var products = await _productService.GetProductsByIdsAsync(cart.Select(s => s.ProductId).ToArray());
                decimal productsPrice = 0;

                foreach (var product in products)
                    productsPrice += product.Price * (cart.FirstOrDefault(f => f.ProductId == product.Id).Quantity);

                var paymentMethodDiscountValue = productsPrice * (decimal)((decimal)paymentMethodDiscount.DiscountPercent / 100);
                result = paymentMethodDiscountValue;
            }

            return result;
        }
        #endregion
    }
}
