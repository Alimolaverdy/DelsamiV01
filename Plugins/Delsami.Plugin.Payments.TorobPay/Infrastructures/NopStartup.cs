using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Payments.TorobPay.Services;
using Nop.Core.Infrastructure;

namespace Delsami.Plugin.Payments.TorobPay.Infrastructures;

public class NopStartup : INopStartup
{
    public int Order => int.MaxValue;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
       // services.AddScoped<ITorobPayAuthService, TorobPayAuthService>();
        services.AddScoped<ITorobPayApiService, TorobPayApiService>();
    }
}
