using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using ZarIran.Plugin.Misc.GoldPurchase.Controllers;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

static void Check(bool valid, string message) { if (!valid) throw new Exception(message); }
var allowed = false;
var calls = 0;
decimal lastDelta = 0;
var permissions = InterfaceProxy.Make<IPermissionService>((method, _) => method.Name == "AuthorizeAsync" ? Task.FromResult(allowed) : throw new Exception(method.Name));
var customers = InterfaceProxy.Make<ICustomerService>((method, _) => method.Name == "GetCustomerByIdAsync" ? Task.FromResult(new Customer { Id = 7 }) : throw new Exception(method.Name));
var work = InterfaceProxy.Make<IWorkContext>((method, _) => method.Name == "GetCurrentCustomerAsync" ? Task.FromResult(new Customer { Id = 99 }) : throw new Exception(method.Name));
var wallet = InterfaceProxy.Make<IGoldWalletService>((method, args) => {
    if (method.Name != "AdjustAsync") throw new Exception(method.Name);
    Check((int)args[0] == 7 && (int)args[3] == 99, "Customer and administrator identity must come from server");
    calls++; lastDelta = (decimal)args[1];
    return Task.FromResult(GoldOperationResult.Fail("test"));
});
var controller = new GoldWalletAdminController(permissions, customers, work, wallet);
await controller.Adjust(7, "1", "reason"); Check(calls == 0, "Unauthorized adjustment must not call wallet");
allowed = true;
foreach (var invalid in new[] { "", "abc", "1.2.3", "1e10" }) { await controller.Adjust(7, invalid, "reason"); Check(calls == 0, "Invalid input must not call wallet"); }
await controller.Adjust(7, "−۰٫۲۵", "correction"); Check(calls == 1 && lastDelta == -.25m, "Persian signed decimal debit");
await controller.Adjust(7, "0,5", "correction"); Check(calls == 2 && lastDelta == .5m, "Decimal comma credit");
var service = new GoldWalletService(null, null, null); // Invalid operations must fail before touching any repository.
foreach (var invalid in new[] { 0m, .000000001m, decimal.MinValue, decimal.MaxValue })
    Check(!(await service.AdjustAsync(7, invalid, "reason", 99)).Success, "Reject zero, excessive precision and out-of-range deltas");
Check(!(await service.AdjustAsync(7, 1, " ", 99)).Success, "Require audit reason");
Check(!(await service.AdjustAsync(7, 1, new string('a', 1001), 99)).Success, "Reject oversized audit reason");
var type = typeof(GoldWalletAdminController);
Check(type.GetCustomAttributes().Any(a => a.GetType().Name == "AuthorizeAdminAttribute"), "Admin authorization filter");
Check(type.GetCustomAttribute<AutoValidateAntiforgeryTokenAttribute>() != null, "Antiforgery filter");
Console.WriteLine("PASS: permissions, admin identity, Persian/decimal parsing, invalid deltas, required audit reason and request filters.");
var settings = new ZarIran.Plugin.Misc.GoldPurchase.GoldPurchaseSettings {
    IsActive = false, DefaultAmount = 2100, MinimumAmount = 100, MaximumAmount = 10000,
    BuyCommission = 5, SellCommission = 3
};
var tradingCustomers = InterfaceProxy.Make<ICustomerService>((method, _) => method.Name == "IsRegisteredAsync" ? Task.FromResult(true) : throw new Exception(method.Name));
var store = InterfaceProxy.Make<IStoreContext>((method, _) => method.Name == "GetCurrentStoreAsync" ? Task.FromResult(new Nop.Core.Domain.Stores.Store { Id = 1 }) : throw new Exception(method.Name));
var readOnlyWallet = InterfaceProxy.Make<IGoldWalletService>((method, _) => method.Name switch {
    "GetBalanceAsync" => Task.FromResult(2m),
    "GetPortfolioAsync" => Task.FromResult(new GoldPortfolio()),
    _ => throw new Exception("Disabled trading must not write wallet: " + method.Name)
});
var trading = new GoldPurchaseController(work, tradingCustomers, null, settings, null, store,
    null, null, null, null, null, null, null, null, null,
    new ZarIran.Plugin.Misc.Core.CoreSettings { ManualGoldPrice = 1000, AllowPriceAdjustment = true, GoldPriceAdjustment = 900, Tax = 90 }, readOnlyWallet, null);
var buyView = (ViewResult)await trading.Buy();
var buyModel = (ZarIran.Plugin.Misc.GoldPurchase.Models.GoldPurchaseModel)buyView.Model;
Check(!buyModel.IsActive && buyModel.BuyCommission == 5 && buyModel.SellCommission == 3, "Model reads activation and commissions from settings");
Check(buyModel.BuyPricePerGram == 1050 && buyModel.GoldWeightGrams == 2, "Buy model includes configured fee");
Check(buyModel.Sell.SellPricePerGram == 970 && buyModel.Sell.CommissionPercent == 3, "Sell model includes configured fee");
Check(await trading.Buy(2) is ViewResult && !trading.ModelState.IsValid, "Disabled buy must return errors without placing an order");
Check(await trading.Sell(1) is ViewResult && !trading.ModelState.IsValid, "Disabled sell must return errors without debiting gold");
Console.WriteLine("PASS: settings drive activation, both quotes, weight calculation and disabled trading rejection.");
settings.IsActive = true;
var productService = InterfaceProxy.Make<Nop.Services.Catalog.IProductService>((method, _) => method.Name == "GetProductByIdAsync" ? Task.FromResult(new Nop.Core.Domain.Catalog.Product { Id = 11, Published = true, CustomerEntersPrice = true }) : throw new Exception(method.Name));
var badPricing = InterfaceProxy.Make<Nop.Services.Orders.IShoppingCartService>((method, _) => method.Name == "GetUnitPriceAsync" ? Task.FromResult((30m, 0m, new List<Nop.Core.Domain.Discounts.Discount>())) : throw new Exception("A mismatched quote must stop before cart/order changes: " + method.Name));
var logger = InterfaceProxy.Make<Nop.Services.Logging.ILogger>((method, _) => method.Name == "ErrorAsync" ? Task.CompletedTask : throw new Exception(method.Name));
var guardedTrading = new GoldPurchaseController(work, tradingCustomers, productService, settings, null, store,
    badPricing, null, null, null, null, null, logger, null, null,
    new ZarIran.Plugin.Misc.Core.CoreSettings { ManualGoldPrice = 1000, AllowPriceAdjustment = true, GoldPriceAdjustment = 900, Tax = 90 }, readOnlyWallet, null);
Check(await guardedTrading.Buy(2) is ViewResult && !guardedTrading.ModelState.IsValid,
    "Price engine returning 30 for a 2100 purchase must be rejected before changing cart or creating order");
Console.WriteLine("PASS: mismatched gold quote stops before cart changes, order placement and gateway.");
foreach (var weight in new[] { .25m, 1m, 2.5m })
{
    var expectedAmount = GoldPricing.GetBuyAmount(weight, 1050m);
    var priceProbe = InterfaceProxy.Make<Nop.Services.Orders.IShoppingCartService>((method, args) => {
        if (method.Name != "GetUnitPriceAsync") throw new Exception("Unexpected cart write");
        Check((decimal)args[6] == expectedAmount, "Server must derive price from exact weight, not fixed one gram");
        return Task.FromResult((30m, 0m, new List<Nop.Core.Domain.Discounts.Discount>()));
    });
    var probe = new GoldPurchaseController(work, tradingCustomers, productService, settings, null, store,
        priceProbe, null, null, null, null, null, logger, null, null,
        new ZarIran.Plugin.Misc.Core.CoreSettings { ManualGoldPrice = 1000, Tax = 90, AllowPriceAdjustment = true, GoldPriceAdjustment = 900 }, readOnlyWallet, null);
    var result = (ViewResult)await probe.Buy(weight);
    var quoteModel = (ZarIran.Plugin.Misc.GoldPurchase.Models.GoldPurchaseModel)result.Model;
    Check(quoteModel.GoldWeightGrams == weight && quoteModel.Amount == expectedAmount,
        "Exact entered weight and spot-plus-fee amount must be retained on errors");
}
Check(!(await service.SellAsync(7, .123456789m, 1000, 2)).Success, "Sell must reject excess precision rather than silently changing the grams");
Console.WriteLine("PASS: .25/1/2.5 grams use spot plus fee; jewelry adjustment and tax do not affect wallet; entered weight remains exact.");

settings.GoldProductId = 11;
var spotForWallet = new ZarIran.Plugin.Misc.Core.CoreSettings { ManualGoldPrice = 1000, AllowPriceAdjustment = true, GoldPriceAdjustment = 900, Tax = 90 };
var cartQuote = new GoldCartQuoteProvider(settings, spotForWallet, work);
Check(await cartQuote.GetUnitPriceAsync(11, 99, 2100) == 2100, "Wallet budget must retain its spot-based amount");
Check(await cartQuote.GetUnitPriceAsync(12, 99, 2100) == null, "Non-wallet products must retain ordinary formula priority");
settings.IsActive = false;
try { await cartQuote.GetUnitPriceAsync(11, 99, 2100); throw new Exception("Disabled quote should fail"); }
catch (InvalidOperationException) { }
settings.IsActive = true;
try { await cartQuote.GetUnitPriceAsync(11, 7, 2100); throw new Exception("Wrong customer quote should fail"); }
catch (InvalidOperationException) { }
Console.WriteLine("PASS: actual wallet quote provider validates spot quote, product identity, activation and customer identity.");




var legacyProduct = new Nop.Core.Domain.Catalog.Product { Id = 11, Published = true };
var updatedLegacyProduct = false;
var legacyProducts = InterfaceProxy.Make<Nop.Services.Catalog.IProductService>((method, _) => method.Name switch
{
    "GetProductByIdAsync" => Task.FromResult(legacyProduct),
    "UpdateProductAsync" => RecordProductUpdate(),
    _ => throw new Exception(method.Name)
});
Task RecordProductUpdate()
{
    updatedLegacyProduct = legacyProduct.CustomerEntersPrice && legacyProduct.MinimumCustomerEnteredPrice == settings.MinimumAmount
        && legacyProduct.MaximumCustomerEnteredPrice == settings.MaximumAmount;
    return Task.CompletedTask;
}
var amountProbe = InterfaceProxy.Make<Nop.Services.Orders.IShoppingCartService>((method, args) =>
{
    Check(method.Name == "GetUnitPriceAsync" && (decimal)args[6] == 2101m && updatedLegacyProduct,
        "Exact budget reaches cart pricing after repairing the configured legacy product");
    return Task.FromResult((30m, 0m, new List<Nop.Core.Domain.Discounts.Discount>()));
});
var amountTrading = new GoldPurchaseController(work, tradingCustomers, legacyProducts, settings, null, store,
    amountProbe, null, null, null, null, null, logger, null, null, spotForWallet, readOnlyWallet, null);
var amountView = (ViewResult)await amountTrading.Buy(999m, purchaseAmount: "۲٬۱۰۱", quoteBy: "amount");
var amountModel = (ZarIran.Plugin.Misc.GoldPurchase.Models.GoldPurchaseModel)amountView.Model;
Check(amountModel.Amount == 2101m && amountModel.GoldWeightGrams == 2.00095238m && amountModel.QuoteBy == "amount",
    "Budget mode ignores the submitted weight and keeps the selected Toman amount");
Console.WriteLine("PASS: editable budget quote and existing gold product repair.");

var saleRate = GoldPricing.GetSellPricePerGram(25250000m, 2m);
var saleWeight = GoldPricing.GetSellWeight(1000000m, saleRate);
Check(GoldPricing.GetSellAmount(saleWeight, saleRate) >= 1000000m &&
    GoldPricing.GetSellAmount(saleWeight - 0.00000001m, saleRate) < 1000000m,
    "A net sale target uses the smallest supported weight that covers the amount after commission");
Console.WriteLine("PASS: sale amounts convert to weight using the net rate and round up to the smallest supported weight.");

var cashWallet = new GoldCashWallet(null, null, null, new RewardPointsSettings { Enabled = true, ExchangeRate = 1 });
Check(cashWallet.ToPoints(101m) == 101 && cashWallet.PointsUpTo(101m, 200) == 101,
    "A 101 Toman gold trade must use 101 wallet points without rounding to tens");
Check(cashWallet.PointsUpTo(301m, 199) == 199, "Split checkout caps the wallet share at its balance");
Check(GoldSheba.Normalize(" ir۰۶۲۹۶۰۰۰۰۰۰۰۱۰۰۳۲۴۲۰۰۰۰۱ ") == "IR062960000000100324200001" &&
    GoldSheba.Normalize("IR062960000000100324200002") == null,
    "Normalize existing Iranian Shebas and reject invalid checksums");
Console.WriteLine("PASS: exact full/partial wallet shares and Iranian Sheba validation.");

public class InterfaceProxy : DispatchProxy
{
    public Func<MethodInfo, object[], object> Handler { get; set; }
    protected override object Invoke(MethodInfo method, object[] args) => Handler(method, args);
    public static T Make<T>(Func<MethodInfo, object[], object> handler) where T : class
    {
        var proxy = Create<T, InterfaceProxy>();
        ((InterfaceProxy)(object)proxy).Handler = handler;
        return proxy;
    }
}
