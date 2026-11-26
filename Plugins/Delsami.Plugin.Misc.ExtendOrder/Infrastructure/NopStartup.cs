using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories;
using Delsami.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core.Infrastructure;
using Nop.Services.ExportImport;
using Nop.Services.Orders;

namespace Delsami.Plugin.Misc.ExtendedOrder.Infrastructure
{
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IExtendedOrderService, ExtendedOrderService>();
            services.AddScoped<IOrderInvoiceService, OrderInvoiceService>();
            services.AddScoped<IDelsamiOrderModelFactory, DelsamiOrderModelFactory>();
            services.AddScoped<ISubStatusService, SubStatusService>();
            services.AddScoped<ISubStatusFactory, SubStatusFactory>();
            services.AddScoped<ISubStatusMappingService, SubStatusMappingService>();
            services.AddScoped<IDelsamiOrderReportService, DelsamiOrderReportService>();
            services.AddScoped<IOrderService, DelsamiOrderService>();
            services.AddScoped<IExportManager, CustomExportManager>();
            services.AddScoped<IWarehouseLocationService, WarehouseLocationService>();
            services.AddScoped<IOrderWarehouseLocationService, OrderWarehouseLocationService>();
        }
        


        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue;
    }
}
