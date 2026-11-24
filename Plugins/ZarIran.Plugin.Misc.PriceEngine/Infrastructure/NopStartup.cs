using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using ZarIran.Plugin.Misc.PriceEngine.Services;

namespace ZarIran.Plugin.Misc.ProductManagement.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 10000;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPriceCalculationService, e>();
        }
    }
}
