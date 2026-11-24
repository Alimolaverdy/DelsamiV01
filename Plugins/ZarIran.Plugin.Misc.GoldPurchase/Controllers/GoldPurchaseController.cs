using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Customers;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ZarIran.Plugin.Misc.ExtendedCustomer.Services;
using ZarIran.Plugin.Misc.Core;
using ZarIran.Plugin.Misc.GoldPurchase.Models;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

namespace ZarIran.Plugin.Misc.GoldPurchase.Controllers;

[AutoValidateAntiforgeryToken]
public class GoldPurchaseController : BasePluginController
{
    private readonly IWorkContext _workContext;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly GoldPurchaseSettings _settings;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IPaymentService _paymentService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IWebHelper _webHelper;
    private readonly ILogger _logger;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly CoreSettings _coreSettings;
    private readonly IGoldWalletService _goldWalletService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly GoldCashWallet _cashWallet;
    private readonly GoldCheckout _goldCheckout;
    private readonly GoldOrderSettlement _settlement;
    private readonly IOrderService _orderService;
    private readonly ICustomerShebaNumberService _shebas;
    private readonly OrderSettings _orderSettings;

    private const string BuyView = "~/Plugins/ZarIran.Misc.GoldPurchase/Views/GoldTrade.cshtml";

    public GoldPurchaseController(
        IWorkContext workContext,
        ICustomerService customerService,
        IProductService productService,
        GoldPurchaseSettings settings,
        ISettingService settingService,
        IStoreContext storeContext,
        IShoppingCartService shoppingCartService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IOrderProcessingService orderProcessingService,
        IGenericAttributeService genericAttributeService,
        IWebHelper webHelper,
        ILogger logger,
        ILocalizationService localizationService,
        INotificationService notificationService,
        CoreSettings coreSettings,
        IGoldWalletService goldWalletService,
        IOrderTotalCalculationService orderTotalCalculationService,
        GoldCashWallet cashWallet = null,
        GoldCheckout goldCheckout = null,
        GoldOrderSettlement settlement = null,
        IOrderService orderService = null,
        ICustomerShebaNumberService shebas = null,
        OrderSettings orderSettings = null)
    {
        _workContext = workContext;
        _customerService = customerService;
        _productService = productService;
        _settings = settings;
        _settingService = settingService;
        _storeContext = storeContext;
        _shoppingCartService = shoppingCartService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _orderProcessingService = orderProcessingService;
        _genericAttributeService = genericAttributeService;
        _webHelper = webHelper;
        _logger = logger;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _coreSettings = coreSettings;
        _goldWalletService = goldWalletService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _cashWallet = cashWallet;
        _goldCheckout = goldCheckout;
        _settlement = settlement;
        _orderService = orderService;
        _shebas = shebas;
        _orderSettings = orderSettings;
    }

    // Wallet gold is spot gold, not the jewelry/product pricing formula.
    private Task<decimal> GetMarketPriceAsync() => Task.FromResult(_coreSettings.ManualGoldPrice);

    [HttpGet]
    public async Task<IActionResult> Buy()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var goldPrice = await GetMarketPriceAsync();

        if (goldPrice <= 0)
            return Content("قیمت لحظه‌ای طلا معتبر نیست؛ تنظیمات نرخ طلا را بررسی کنید.");

        var model = await PrepareBuyModelAsync(customer, _settings.DefaultAmount, goldPrice);

        return View(BuyView, model);
    }

    [HttpPost]
    public async Task<IActionResult> Buy(decimal goldWeightGrams, bool useCashWallet = false, decimal? cashWalletAmount = null, string purchaseAmount = null, string quoteBy = null)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var store = await _storeContext.GetCurrentStoreAsync();
        var goldPrice = await GetMarketPriceAsync();
        var buyPrice = GoldPricing.GetBuyPricePerGram(goldPrice, _settings.BuyCommission);
        var goldWeight = goldWeightGrams;
        if (useCashWallet && cashWalletAmount.HasValue &&
            (cashWalletAmount.Value <= 0 || decimal.Truncate(cashWalletAmount.Value) != cashWalletAmount.Value))
            ModelState.AddModelError(nameof(cashWalletAmount), "مبلغ کیف پول را به صورت عدد صحیح و مثبت وارد کنید.");
        var byAmount = quoteBy == "amount";
        ModelState.Remove(byAmount ? nameof(goldWeightGrams) : nameof(purchaseAmount));
        if (byAmount)
            goldWeight = 0m;
        var amount = 0m;
        if (byAmount)
        {
            if (!GoldPricing.TryParseTomanAmount(purchaseAmount, out amount) || amount <= 0 || amount > _settings.MaximumAmount)
                ModelState.AddModelError(nameof(purchaseAmount), "مبلغ خرید را به صورت عدد صحیح و در بازه مجاز وارد کنید.");
            else if (buyPrice > 0)
                goldWeight = GoldPricing.GetBuyWeight(amount, buyPrice);
        }

        if (goldWeight <= 0 || goldWeight > 9999999999m || goldWeight != GoldPricing.RoundGrams(goldWeight))
            ModelState.AddModelError(nameof(goldWeightGrams), "وزن خرید باید مثبت و حداکثر با ۸ رقم اعشار باشد.");
        else if (!byAmount)
        {
            try { amount = GoldPricing.GetBuyAmount(goldWeight, buyPrice); }
            catch (OverflowException) { ModelState.AddModelError(nameof(goldWeightGrams), "مقدار خرید خارج از محدوده مجاز است."); }
        }

        async Task<IActionResult> ShowBuyErrorAsync()
        {
            var model = await PrepareBuyModelAsync(customer, amount, goldPrice, goldWeight);
            model.QuoteBy = byAmount ? "amount" : "weight";
            model.UseCashWallet = useCashWallet;
            model.CashWalletAmount = cashWalletAmount;
            return View(BuyView, model);
        }

        if (!_settings.IsActive)
            ModelState.AddModelError(nameof(amount), "خرید و فروش طلا غیر فعال است.");

        if (goldPrice <= 0)
            ModelState.AddModelError(nameof(amount), "قیمت لحظه‌ای طلا معتبر نیست؛ تنظیمات نرخ طلا را بررسی کنید.");

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "مبلغ خرید باید بیشتر از صفر باشد.");

        if (amount < _settings.MinimumAmount)
            ModelState.AddModelError(nameof(amount), $"حداقل مبلغ خرید {_settings.MinimumAmount:N0} تومان است.");

        if (amount > _settings.MaximumAmount)
            ModelState.AddModelError(nameof(amount), $"حداکثر مبلغ خرید {_settings.MaximumAmount:N0} تومان است.");

        if (_settings.BuyCommission < 0 || _settings.BuyCommission > 100)
            ModelState.AddModelError(nameof(amount), "کارمزد خرید در تنظیمات معتبر نیست.");

        if (!ModelState.IsValid)
            return await ShowBuyErrorAsync();

        var product = await _productService.GetProductByIdAsync(_settings.GoldProductId);
        if (product == null || !product.Published || product.Deleted)
        {
            ModelState.AddModelError(nameof(amount), "محصول خرید طلا تنظیم نشده است.");
            return await ShowBuyErrorAsync();
        }

        if (!product.CustomerEntersPrice)
        {
            // The configured trading product must persist its server-calculated amount through nopCommerce.
            product.CustomerEntersPrice = true;
            product.MinimumCustomerEnteredPrice = _settings.MinimumAmount;
            product.MaximumCustomerEnteredPrice = _settings.MaximumAmount;
            await _productService.UpdateProductAsync(product);
        }

        var quote = await _shoppingCartService.GetUnitPriceAsync(product, customer, store,
            ShoppingCartType.ShoppingCart, 1, string.Empty, amount, null, null, true);
        if (quote.unitPrice != amount)
        {
            await _logger.ErrorAsync($"Gold purchase blocked for customer {customer.Id}: requested {amount}, calculated unit price {quote.unitPrice}, product {product.Id}.");
            ModelState.AddModelError(nameof(amount), "مبلغ محصول با مبلغ خرید طلا مطابقت ندارد. سفارش ثبت نشد؛ لطفاً با پشتیبانی تماس بگیرید.");
            return await ShowBuyErrorAsync();
        }

        var cart = await _shoppingCartService.GetShoppingCartAsync(
            customer, ShoppingCartType.ShoppingCart, store.Id);

        foreach (var item in cart)
            await _shoppingCartService.DeleteShoppingCartItemAsync(item);

        var warnings = await _shoppingCartService.AddToCartAsync(
            customer,
            product,
            ShoppingCartType.ShoppingCart,
            store.Id,
            attributesXml: string.Empty,
            customerEnteredPrice: amount,
            quantity: 1);

        if (warnings.Any())
        {
            foreach (var warning in warnings)
                ModelState.AddModelError(nameof(amount), warning);

            return await ShowBuyErrorAsync();
        }

        Order order;
        IList<string> errors;
        try
        {
            (order, errors) = await PlaceGoldOrderAsync(customer, store.Id, amount, buyPrice, goldWeight,
                useCashWallet, cashWalletAmount);
        }
        catch (Exception error) when (error is DbException or TransactionException)
        {
            await _logger.ErrorAsync($"Could not commit gold order for customer {customer.Id}.", error);
            order = null;
            errors = new[] { "ثبت سفارش کامل نشد. دوباره تلاش کنید." };
        }
        if (order == null)
        {
            foreach (var error in errors)
                ModelState.AddModelError(nameof(amount), error);
            return await ShowBuyErrorAsync();
        }

        HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);

        if (order.PaymentStatus != PaymentStatus.Paid)
        {
            await _paymentService.PostProcessPaymentAsync(new PostProcessPaymentRequest { Order = order });

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone || Response.HasStarted ||
                (Response.StatusCode is >= 300 and < 400 && Response.Headers.ContainsKey("Location")))
                return new EmptyResult();

            return RedirectToRoute("OrderDetails", new { orderId = order.Id });
        }

        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
    }

    private async Task<(Order Order, IList<string> Errors)> PlaceGoldOrderAsync(
        Customer customer, int storeId, decimal amount, decimal buyPrice, decimal goldWeight,
        bool useCashWallet, decimal? requestedWalletAmount)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);
        if (cart.Count != 1 || cart[0].ProductId != _settings.GoldProductId ||
            cart[0].CustomerEnteredPrice != amount || cart[0].Quantity != 1)
            return (null, new[] { "سبد خرید طلا تغییر کرده است. دوباره تلاش کنید." });

        var walletPoints = 0;
        if (useCashWallet)
        {
            var available = await _cashWallet.GetRewardPointsBalanceAsync(customer.Id, storeId);
            walletPoints = requestedWalletAmount.HasValue
                ? _cashWallet.ToPoints(requestedWalletAmount.Value)
                : _cashWallet.PointsUpTo(amount, available);

            if (walletPoints <= 0 || walletPoints > available || _cashWallet.ToAmount(walletPoints) > amount)
                return (null, new[] { "مبلغ پرداخت از کیف پول باید در محدوده موجودی و مبلغ خرید باشد." });
        }

        var walletAmount = _cashWallet.ToAmount(walletPoints);
        var gatewayAmount = amount - walletAmount;
        var request = new ProcessPaymentRequest { PaymentMethodSystemName = string.Empty };
        if (gatewayAmount > 0)
        {
            var gateway = await _paymentPluginManager.LoadPluginBySystemNameAsync(_settings.PaymentSystemName, customer, storeId);
            if (gateway == null || !_paymentPluginManager.IsPluginActive(gateway) ||
                gateway.PaymentMethodType != PaymentMethodType.Redirection)
                return (null, new[] { "درگاه پرداخت فعال نیست." });
            if (await _paymentService.GetAdditionalHandlingFeeAsync(cart, _settings.PaymentSystemName) != 0)
                return (null, new[] { "درگاه خرید طلا نباید هزینه پرداخت اضافه داشته باشد." });

            var warnings = await gateway.ValidatePaymentFormAsync(null);
            if (warnings.Any())
                return (null, warnings);
            request = await gateway.GetPaymentInfoAsync(null) ?? new ProcessPaymentRequest();
            request.PaymentMethodSystemName = _settings.PaymentSystemName;
        }

        var total = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart,
            useRewardPoints: false, usePaymentMethodAdditionalFee: false);
        if (total.shoppingCartTotal != amount || total.appliedGiftCards.Count != 0 ||
            total.discountAmount != 0 || total.appliedDiscounts.Count != 0)
            return (null, new[] { "مبلغ نهایی سفارش با قیمت خرید طلا برابر نیست. تنظیمات محصول و هزینه‌های سفارش را بررسی کنید." });

        var minimumOrderAmount = _orderSettings.MinOrderTotalAmount;
        if (amount < minimumOrderAmount)
            return (null, new[] { $"حداقل مبلغ سفارش فروشگاه {minimumOrderAmount:N0} تومان است." });

        request.StoreId = storeId;
        request.CustomerId = customer.Id;
        request.OrderGuid = Guid.NewGuid();
        request.OrderGuidGeneratedOnUtc = DateTime.UtcNow;

        _goldCheckout.CartItemId = cart[0].Id;
        _goldCheckout.GrossAmount = amount;
        _goldCheckout.CashAmount = walletAmount;
        // This settings instance is request scoped. The minimum was checked against the full price above.
        // The built-in checkout otherwise compares the minimum to the gateway remainder.
        _orderSettings.MinOrderTotalAmount = 0;
        PlaceOrderResult result;
        try
        {
            result = await _orderProcessingService.PlaceOrderAsync(request);
        }
        finally
        {
            _goldCheckout.CartItemId = 0;
            _orderSettings.MinOrderTotalAmount = minimumOrderAmount;
        }

        if (!result.Success)
            return (null, result.Errors);
        if (result.PlacedOrder == null)
            return (null, new[] { "ثبت سفارش کامل نشد. دوباره تلاش کنید." });

        var order = result.PlacedOrder;
        if (order.OrderTotal != gatewayAmount)
            return (null, new[] { "مبلغ سفارش با مبلغ پرداخت مطابقت ندارد. دوباره تلاش کنید." });

        if (walletPoints > 0)
        {
            order.RedeemedRewardPointsEntryId = await _settlement.RecordAsync(order, customer, walletPoints, walletAmount);
            await _orderService.UpdateOrderAsync(order);
        }

        await _genericAttributeService.SaveAttributeAsync(order, GoldPurchaseDefaults.OrderAmountAttribute, amount);
        await _genericAttributeService.SaveAttributeAsync(order, GoldPurchaseDefaults.OrderGoldPriceAttribute, buyPrice);
        await _genericAttributeService.SaveAttributeAsync(order, GoldPurchaseDefaults.OrderGoldWeightAttribute, goldWeight);

        // nopCommerce publishes OrderPaid inside PlaceOrderAsync for zero-total orders, before these attributes exist.
        if (order.PaymentStatus == PaymentStatus.Paid)
            await _goldWalletService.CreditPurchaseAsync(customer.Id, order.Id, amount, buyPrice, goldWeight);

        scope.Complete();
        return (order, null);
    }

    private async Task<GoldPurchaseModel> PrepareBuyModelAsync(
        Nop.Core.Domain.Customers.Customer customer, decimal amount, decimal goldPrice, decimal? requestedWeight = null)
    {
        var buyPrice = GoldPricing.GetBuyPricePerGram(goldPrice, _settings.BuyCommission);
        var weight = requestedWeight ?? GoldPricing.GetBuyWeight(amount, buyPrice);
        var cashBalance = _cashWallet == null ? 0 :
            _cashWallet.ToAmount(await _cashWallet.GetRewardPointsBalanceAsync(customer.Id,
                (await _storeContext.GetCurrentStoreAsync()).Id));
        var shebaNumbers = Array.Empty<string>();
        if (_shebas != null)
        {
            var shebas = await _shebas.GetCustomerShebaNumbersAsync(customer.Id);
            shebaNumbers = shebas.Select(item => GoldSheba.Normalize(item.Number))
                .Where(number => number != null).Distinct().ToArray();
        }
        return new GoldPurchaseModel
        {
            CashWalletBalance = cashBalance,
            Shebas = shebaNumbers,
            Amount = amount,
            GoldPricePerGram = goldPrice,
            BuyPricePerGram = GoldPricing.GetBuyPricePerGram(goldPrice, _settings.BuyCommission),
            BuyCommission = _settings.BuyCommission,
            SellCommission = _settings.SellCommission,
            GoldWeightGrams = weight,
            MinimumAmount = _settings.MinimumAmount,
            MaximumAmount = _settings.MaximumAmount,
            PaymentSystemName = _settings.PaymentSystemName,
            WalletBalanceGrams = await _goldWalletService.GetBalanceAsync(customer.Id),
            IsActive = _settings.IsActive,
            Sell = await PrepareSellModelAsync(customer, 0),
            Portfolio = await _goldWalletService.GetPortfolioAsync(customer.Id, goldPrice)
        };
    }

    private async Task<GoldSellModel> PrepareSellModelAsync(
        Nop.Core.Domain.Customers.Customer customer, decimal grams)
    {
        var market = await GetMarketPriceAsync();

        return new GoldSellModel
        {
            GoldWeightGrams = grams,
            MarketPricePerGram = market,
            CommissionPercent = _settings.SellCommission,
            SellPricePerGram = GoldPricing.GetSellPricePerGram(market, _settings.SellCommission),
            WalletBalanceGrams = await _goldWalletService.GetBalanceAsync(customer.Id)
        };
    }

    private async Task<GoldPurchaseModel> PrepareTradingSellModelAsync(Nop.Core.Domain.Customers.Customer customer, decimal grams)
    {
        var model = await PrepareBuyModelAsync(customer, _settings.DefaultAmount, await GetMarketPriceAsync());
        model.Sell.GoldWeightGrams = grams;
        model.ActiveTab = "sell";
        return model;
    }

    [HttpGet]
    public async Task<IActionResult> Sell()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if ((await GetMarketPriceAsync()) <= 0)
            return Content("قیمت لحظه‌ای طلا معتبر نیست؛ تنظیمات نرخ طلا را بررسی کنید.");

        var model = await PrepareTradingSellModelAsync(customer, 0);
        model.SuccessMessage = (TempData["ZarIran.GoldSell.Success"] ?? TempData["GoldPayout.Message"]) as string;
        if (TempData["GoldPayout.Error"] is string error)
            ModelState.AddModelError(string.Empty, error);

        return View(BuyView, model);
    }

    [HttpPost]
    public async Task<IActionResult> Sell(decimal goldWeightGrams, string saleAmount = null, string sellQuoteBy = null)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var marketPrice = await GetMarketPriceAsync();
        var byAmount = sellQuoteBy == "amount";
        decimal? targetAmount = null;
        if (byAmount)
        {
            ModelState.Remove(nameof(goldWeightGrams));
            goldWeightGrams = 0m;
            var price = GoldPricing.GetSellPricePerGram(marketPrice, _settings.SellCommission);
            var balance = await _goldWalletService.GetBalanceAsync(customer.Id);
            if (!GoldPricing.TryParseTomanAmount(saleAmount, out var requested) || requested <= 0)
                ModelState.AddModelError(nameof(saleAmount), "مبلغ دریافتی را به صورت عدد صحیح و مثبت وارد کنید.");
            else
            {
                targetAmount = requested;
                if (requested > GoldPricing.GetSellAmount(balance, price))
                    ModelState.AddModelError(nameof(saleAmount), "مبلغ دریافتی از ارزش قابل فروش موجودی طلای شما بیشتر است.");
                else
                    goldWeightGrams = GoldPricing.GetSellWeight(requested, price);
            }
        }

        if (!_settings.IsActive)
            ModelState.AddModelError(nameof(goldWeightGrams), "خرید و فروش طلا غیر فعال است.");

        if (marketPrice <= 0)
            ModelState.AddModelError(nameof(goldWeightGrams), "قیمت لحظه‌ای طلا معتبر نیست؛ تنظیمات نرخ طلا را بررسی کنید.");

        if (_settings.SellCommission < 0 || _settings.SellCommission > 100)
            ModelState.AddModelError(nameof(goldWeightGrams), "کارمزد فروش در تنظیمات معتبر نیست.");

        if (ModelState.IsValid)
        {
            // The price is read again on the server at the moment of the sale; the browser's number is only a preview.
            GoldOperationResult result;
            try
            {
                result = await _goldWalletService.SellAsync(
                    customer.Id,
                    goldWeightGrams,
                    marketPrice,
                    _settings.SellCommission,
                    (await _storeContext.GetCurrentStoreAsync()).Id);
            }
            catch (Exception error) when (error is DbException or TransactionException)
            {
                await _logger.ErrorAsync($"Could not commit gold sale for customer {customer.Id}.", error);
                result = GoldOperationResult.Fail("ثبت فروش کامل نشد. دوباره تلاش کنید.");
            }

            if (result.Success)
            {
                var t = result.Transaction;
                TempData["ZarIran.GoldSell.Success"] =
                    $"{(-t.GoldWeightGrams).ToString("0.########", System.Globalization.CultureInfo.InvariantCulture)} گرم طلا به قیمت هر گرم {t.PricePerGram:N0} تومان فروخته شد. مبلغ {t.Amount:N0} تومان به کیف پول شما واریز شد.";

                return RedirectToRoute(GoldPurchaseDefaults.SellRoute);
            }

            ModelState.AddModelError(nameof(goldWeightGrams), result.Error);
        }

        var model = await PrepareTradingSellModelAsync(customer, goldWeightGrams);
        model.Sell.QuoteBy = byAmount ? "amount" : "weight";
        model.Sell.TargetAmount = targetAmount;
        return View(BuyView, model);
    }

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public async Task<IActionResult> Configure()
    {
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<GoldPurchaseSettings>(storeId);

        return View("~/Plugins/ZarIran.Misc.GoldPurchase/Views/Configure.cshtml",
            new ConfigurationModel
            {
                ActiveStoreScopeConfiguration = storeId,
                GoldProductId = settings.GoldProductId,
                DefaultAmount = settings.DefaultAmount,
                MinimumAmount = settings.MinimumAmount,
                MaximumAmount = settings.MaximumAmount,
                PaymentSystemName = settings.PaymentSystemName,
                IsActive = settings.IsActive,
                BuyCommission = settings.BuyCommission,
                SellCommission = settings.SellCommission,
            });
    }

    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (model.BuyCommission < 0 || model.BuyCommission > 100)
            ModelState.AddModelError(nameof(model.BuyCommission), "کارمزد خرید باید بین صفر و ۱۰۰ درصد باشد.");
        if (model.SellCommission < 0 || model.SellCommission > 100)
            ModelState.AddModelError(nameof(model.SellCommission), "کارمزد فروش باید بین صفر و ۱۰۰ درصد باشد.");
        if (model.MinimumAmount <= 0 || model.MaximumAmount < model.MinimumAmount)
            ModelState.AddModelError(nameof(model.MinimumAmount), "بازه مبلغ خرید معتبر نیست.");
        if (model.DefaultAmount < model.MinimumAmount || model.DefaultAmount > model.MaximumAmount)
            ModelState.AddModelError(nameof(model.DefaultAmount), "مبلغ پیش‌فرض باید داخل بازه مجاز باشد.");
        if (!ModelState.IsValid)
            return await Configure();

        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<GoldPurchaseSettings>(storeId);

        settings.GoldProductId = model.GoldProductId;
        settings.DefaultAmount = model.DefaultAmount;
        settings.MinimumAmount = model.MinimumAmount;
        settings.MaximumAmount = model.MaximumAmount;
        settings.PaymentSystemName = model.PaymentSystemName;
        settings.IsActive = model.IsActive;
        settings.BuyCommission = model.BuyCommission;
        settings.SellCommission = model.SellCommission;

        await _settingService.SaveSettingAsync(settings, storeId);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }
}
