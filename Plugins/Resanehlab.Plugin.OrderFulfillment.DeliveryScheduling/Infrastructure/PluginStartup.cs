using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Factories;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Helpers;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Services;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure
{
    public class PluginStartup : INopStartup
    {
        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddScoped<IAvailableDeliveryTimeRangeService, AvailableDeliveryTimeRangeService>();
            services.AddScoped<IShippingMethodCapacityService, ShippingMethodCapacityService>();
            services.AddScoped<IExceptionDateService, ExceptionDateService>();
            services.AddScoped<IOrderShippingMethodCapacityMappingService, OrderShippingMethodCapacityMappingService>();
            services.AddScoped<IAvailableDeliveryDateTimeRangeService, AvailableDeliveryDateTimeRangeService>();
            services.AddScoped<IOverriddenStartDisplayDaysOffsetService, OverriddenStartDisplayDaysOffsetService>();
            services.AddScoped<IAvailableDeliveryDateTimeTableModelFactory, AvailableDeliveryDateTimeTableModelFactory>();
            services.AddScoped<IRLSqlProvider, MSSqlProvider>();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 11;
    }
}
