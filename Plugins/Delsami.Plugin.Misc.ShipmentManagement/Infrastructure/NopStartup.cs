using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.ShipmentManagement.Areas.Admin.Factories;
using ZarIran.Plugin.Misc.ShipmentManagement.Factories;
using ZarIran.Plugin.Misc.ShipmentManagement.Infrastructure.Permission;
using ZarIran.Plugin.Misc.ShipmentManagement.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Security;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Infrastructure
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
            services.AddScoped<IShipmentDeliveryService, ShipmentDeliveryService>();
            services.AddScoped<IZarIranShipmentModelFactory, ZarIranShipmentModelFactory>();
            services.AddScoped<IZarIranOrderModelFactory, ZarIranOrderModelFactory>();
            services.AddScoped<IShipmentTrackingImportService, ShipmentTrackingImportService>();
            services.AddScoped<IPermissionProvider, ZarIranCustomPermissionForImportFile>();


            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ShipmentManagementViewLocationExpander());
            });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            //services.AddVersionedApiExplorer(options =>
            //{
            //    options.GroupNameFormat = "'v'VVV";
            //    options.SubstituteApiVersionInUrl = true;
            //});

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
        public int Order => 1001;
    }
}