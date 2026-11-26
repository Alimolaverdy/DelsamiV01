using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Shipping.IRPost.Areas.Admin.Factories;
using Delsami.Plugin.Shipping.IRPost.Services;
using Nop.Core.Infrastructure;

namespace Delsami.Plugin.Shipping.IRPost.Infrastructures
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IIRPostService, IRPostService>();
            services.AddScoped<IPriceRangeService, PriceRangeService>();
            services.AddScoped<IPriceRangeModelFactory, PriceRangeModelFactory>();
            services.AddScoped<IDelsamiPostCityMappingService, DelsamiPostCityMappingService>();
        }
    }
}
