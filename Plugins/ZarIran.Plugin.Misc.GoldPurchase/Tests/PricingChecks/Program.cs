using System.Reflection;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using ZarIran.Plugin.Misc.ProductManagements.Services;

var parser = Proxy.Make<IProductAttributeParser>((method, _) => method.Name switch {
    "FindProductAttributeCombinationAsync" => Task.FromResult<ProductAttributeCombination>(null),
    "ParseProductAttributeValuesAsync" => Task.FromResult<IList<ProductAttributeValue>>(new List<ProductAttributeValue>()),
    _ => throw new Exception(method.Name)
});
var mapping = Proxy.Make<IVirtualProductMappingService>((method, _) => method.Name == "GetMainProductByVirtualProductIdAsync" ? Task.FromResult((false, (Product)null)) : throw new Exception(method.Name));
var metadata = Proxy.Make<IProductPricingService>((method, _) => method.Name == "GetByProductIdAsync" ? Task.FromResult(new ProductPricing()) : throw new Exception(method.Name));
var calculated = Proxy.Make<IZarIranPriceCalculationService>((method, _) => method.Name == "GetCalculatedProductPriceAsync" ? Task.FromResult((30m, 30m)) : throw new Exception(method.Name));
var providers = new List<IGoldCartQuoteProvider>();
var constructor = typeof(ZarIranShoppingCartService).GetConstructors().Single();
var arguments = constructor.GetParameters().Select(p => p.ParameterType switch {
    var t when t == typeof(IProductAttributeParser) => (object)parser,
    var t when t == typeof(IVirtualProductMappingService) => mapping,
    var t when t == typeof(IProductPricingService) => metadata,
    var t when t == typeof(IZarIranPriceCalculationService) => calculated,
    var t when t == typeof(IEnumerable<IGoldCartQuoteProvider>) => providers,
    var t when t == typeof(ShoppingCartSettings) => new ShoppingCartSettings { RoundPricesDuringCalculation = false },
    _ => null
}).ToArray();
var service = (ZarIranShoppingCartService)constructor.Invoke(arguments);
var product = new Product { Id = 11, CustomerEntersPrice = true, ProductType = ProductType.SimpleProduct, Weight = 0 };
var quote = await service.GetUnitPriceAsync(product, new Customer(), new Store(), ShoppingCartType.ShoppingCart, 1, "", 25000000, null, null, true);
if (quote.unitPrice != 30) throw new Exception("Formula must retain priority for ordinary customer-priced products.");
providers.Add(Proxy.Make<IGoldCartQuoteProvider>((method, args) => Task.FromResult<decimal?>((int)args[0] == 11 ? (decimal)args[2] : null)));
quote = await service.GetUnitPriceAsync(product, new Customer(), new Store(), ShoppingCartType.ShoppingCart, 1, "", 25000000, null, null, true);
if (quote.unitPrice != 25000000) throw new Exception("Validated wallet formula quote was replaced by fixed product weight.");
product.Id = 12;
quote = await service.GetUnitPriceAsync(product, new Customer(), new Store(), ShoppingCartType.ShoppingCart, 1, "", 25000000, null, null, true);
if (quote.unitPrice != 30) throw new Exception("Non-wallet product formula must not change.");
// The normal formula uses Product.Weight and adjustments exactly as before.
product.Weight = .25m;
var productService = Proxy.Make<IProductService>((method, _) => Task.FromResult(product));
var formulaMetadata = Proxy.Make<IProductPricingService>((method, _) => Task.FromResult(new ProductPricing { AdditionalAmount = 30 }));
var work = Proxy.Make<Nop.Core.IWorkContext>((method, _) => Task.FromResult(new Customer()));
var customers = Proxy.Make<Nop.Services.Customers.ICustomerService>((method, _) => Task.FromResult(Array.Empty<int>()));
var engine = new ZarIranPriceCalculationService(productService, new ZarIran.Plugin.Misc.Core.CoreSettings {
    ManualGoldPrice = 25000000, AllowPriceAdjustment = true, GoldPriceAdjustment = 1000000
}, formulaMetadata, work, customers);
if ((await engine.GetCalculatedProductPriceAsync(product.Id)).Price != 6001000) throw new Exception("Normal product price must use the weight and adjustment, then round up.");
if (ProductPriceRounding.RoundUp(76658250m) != 76659000m ||
    ProductPriceRounding.RoundUp(76659000m) != 76659000m ||
    ProductPriceRounding.RoundUp(0m) != 0m)
    throw new Exception("Product prices must round up to the next multiple of 1,000.");
Console.WriteLine("PASS: normal formula priority and actual product weight preserved; only wallet purchase keeps its own quoted total.");
public class Proxy : DispatchProxy {
    public Func<MethodInfo, object[], object> Handler { get; set; }
    protected override object Invoke(MethodInfo method, object[] args) => Handler(method, args);
    public static T Make<T>(Func<MethodInfo, object[], object> handler) where T:class {
        var proxy = Create<T, Proxy>(); ((Proxy)(object)proxy).Handler = handler; return proxy;
    }
}
