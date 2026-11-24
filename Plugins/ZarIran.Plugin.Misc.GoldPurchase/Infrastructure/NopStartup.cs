using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Orders;
using ZarIran.Plugin.Misc.GoldPurchase.Services;

namespace ZarIran.Plugin.Misc.GoldPurchase.Infrastructure;

public class NopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<GoldCashWallet>();
        services.AddScoped<GoldWithdrawalService>();
        services.AddScoped<GoldOrderSettlement>();
        services.AddScoped<GoldCheckout>();
        services.AddScoped<IOrderTotalCalculationService, GoldCheckoutTotalService>();
        services.AddScoped<IGoldWalletService, GoldWalletService>();
        services.AddScoped<ZarIran.Plugin.Misc.Core.Services.IGoldCartQuoteProvider, GoldCartQuoteProvider>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 30000;
}
