using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Shipping.ZarIranLogistic.Services;
using Nop.Core.Infrastructure;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Infrastructures
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IZarIranLogisticService, ZarIranLogisticService>();
            services.AddScoped<IZarIranLogisticApiService, ZarIranLogisticApiService>();
            services.AddScoped<IQueuedCanceledShipmentForSendToLogisticService, QueuedCanceledShipmentForSendToLogisticService>();
            
        }
    }
}
