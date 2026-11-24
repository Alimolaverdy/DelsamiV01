using System;
using System.Threading.Tasks;
using Nop.Core;
using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.Core;
namespace ZarIran.Plugin.Misc.GoldPurchase.Services;
/// <summary>Preserve the spot-gold purchase total in cart/order pricing; never apply the product formula.</summary>
public class GoldCartQuoteProvider : IGoldCartQuoteProvider
{
    private readonly GoldPurchaseSettings _settings;
    private readonly CoreSettings _coreSettings;
    private readonly IWorkContext _workContext;
    public GoldCartQuoteProvider(GoldPurchaseSettings settings, CoreSettings coreSettings, IWorkContext workContext)
    { _settings = settings; _coreSettings = coreSettings; _workContext = workContext; }
    public async Task<decimal?> GetUnitPriceAsync(int productId, int customerId, decimal enteredAmount)
    {
        if (productId != _settings.GoldProductId) return null;
        if (!_settings.IsActive || _settings.BuyCommission < 0 || _settings.BuyCommission > 100 ||
            enteredAmount <= 0 || enteredAmount < _settings.MinimumAmount || enteredAmount > _settings.MaximumAmount)
            throw new InvalidOperationException("تنظیمات یا مبلغ خرید طلا معتبر نیست.");
        if ((await _workContext.GetCurrentCustomerAsync()).Id != customerId)
            throw new InvalidOperationException("مشتری خرید طلا معتبر نیست.");
        var market = _coreSettings.ManualGoldPrice;
        var price = GoldPricing.GetBuyPricePerGram(market, _settings.BuyCommission);
        if (market <= 0 || price <= 0 || GoldPricing.RoundGrams(enteredAmount / price) <= 0)
            throw new InvalidOperationException("قیمت لحظه‌ای طلا معتبر نیست.");
        return enteredAmount;
    }
}
