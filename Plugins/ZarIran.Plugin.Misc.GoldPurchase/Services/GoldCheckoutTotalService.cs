using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;

namespace ZarIran.Plugin.Misc.GoldPurchase.Services;

public class GoldCheckout
{
    public int CartItemId { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal CashAmount { get; set; }
}

public class GoldCheckoutTotalService : OrderTotalCalculationService
{
    private readonly GoldCheckout _checkout;

    public GoldCheckoutTotalService(GoldCheckout checkout, CatalogSettings catalogSettings,
        IAddressService addressService, ICheckoutAttributeParser checkoutAttributeParser,
        ICustomerService customerService, IDiscountService discountService, IGenericAttributeService genericAttributeService,
        IGiftCardService giftCardService, IOrderService orderService, IPaymentService paymentService,
        IPriceCalculationService priceCalculationService, IProductService productService, IRewardPointService rewardPointService,
        IShippingPluginManager shippingPluginManager, IShippingService shippingService, IShoppingCartService shoppingCartService,
        IStoreContext storeContext, ITaxService taxService, IWorkContext workContext, RewardPointsSettings rewardPointsSettings,
        ShippingSettings shippingSettings, ShoppingCartSettings shoppingCartSettings, TaxSettings taxSettings)
        : base(catalogSettings, addressService, checkoutAttributeParser, customerService, discountService, genericAttributeService,
            giftCardService, orderService, paymentService, priceCalculationService, productService, rewardPointService,
            shippingPluginManager, shippingService, shoppingCartService, storeContext, taxService, workContext,
            rewardPointsSettings, shippingSettings, shoppingCartSettings, taxSettings)
    {
        _checkout = checkout;
    }

    public override Task<(decimal taxTotal, SortedDictionary<decimal, decimal> taxRates)>
        GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
        => base.GetTaxTotalAsync(cart, _checkout.CartItemId == 0 && usePaymentMethodAdditionalFee);

    public override async Task<(decimal? shoppingCartTotal, decimal discountAmount, List<Discount> appliedDiscounts,
        List<AppliedGiftCard> appliedGiftCards, int redeemedRewardPoints, decimal redeemedRewardPointsAmount)>
        GetShoppingCartTotalAsync(IList<ShoppingCartItem> cart, bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true)
    {
        if (_checkout.CartItemId == 0)
            return await base.GetShoppingCartTotalAsync(cart, useRewardPoints, usePaymentMethodAdditionalFee);

        if (cart.Count != 1 || cart[0].Id != _checkout.CartItemId || cart[0].Quantity != 1)
            throw new InvalidOperationException("سبد خرید طلا تغییر کرده است. دوباره تلاش کنید.");

        var total = await base.GetShoppingCartTotalAsync(cart, false, false);
        if (total.shoppingCartTotal != _checkout.GrossAmount || total.appliedGiftCards.Count != 0 ||
            total.discountAmount != 0 || total.appliedDiscounts.Count != 0)
            throw new InvalidOperationException("مبلغ سبد خرید با قیمت طلا مطابقت ندارد.");

        total.shoppingCartTotal -= _checkout.CashAmount;
        return total;
    }
}
