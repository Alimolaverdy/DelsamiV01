using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using Nop.Core.Infrastructure;
using Nop.Services.ExportImport;
using Nop.Services.Orders;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Infrastructure
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
            services.AddScoped<IZarIranOrderModelFactory, ZarIranOrderModelFactory>();
            services.AddScoped<ISubStatusService, SubStatusService>();
            services.AddScoped<ISubStatusFactory, SubStatusFactory>();
            services.AddScoped<ISubStatusMappingService, SubStatusMappingService>();
            services.AddScoped<IZarIranOrderReportService, ZarIranOrderReportService>();
            services.AddScoped<IOrderService, ZarIranOrderService>();
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
