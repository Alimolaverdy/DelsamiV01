using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.Core.Services;
using Nop.Core.Infrastructure;
using ZarIran.Plugin.Misc.Core.Helpers;
using ZarIran.Plugin.Misc.Core.Models.Resilience;
using ZarIran.Plugin.Misc.Core.Services.ApiCaller;
using ZarIran.Plugin.Misc.Core.Services.CityService;
using ZarIran.Plugin.Misc.Core.Services.Resilience;
using ZarIran.Plugin.Misc.Core.Services.ShippingMethod;
using ZarIran.Plugin.Misc.Core.Services.ShippingMethodService;
using Nop.Data;
using ZarIran.Plugin.Misc.Core.Infrastrcuture.Data;

namespace ZarIran.Plugin.Misc.Core.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 10000;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IApiCaller, ApiCaller>();
            services.AddScoped<ICityService, CityService>();
            services.AddSingleton(new ResilienceOptions());
            services.AddScoped<IResilienceService, ResilienceService>();
            services.AddScoped<IShippingMethodService, ShippingMethodService>();
            services.AddScoped<ICustomCustomerAttributesXMLService, CustomCustomerAttributesXMLService>();
            services.AddScoped<ICustomerAttributeValidationService, CustomerAttributeValidationService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IZarIranCustomerService, ZarIranCustomerService>();
            services.AddScoped<IEnumHelper, EnumHelper>();
            services.AddScoped<IVirtualProductMappingService, VirtualProductMappingService>();
            services.AddScoped<IProductStockHistoryService, ProductStockHistoryService>();
            services.AddScoped<IImportReportService, ImportReportService>();

            services.AddScoped<
            INopDataProvider,
            ZarIranMsSqlDataProvider>();
        }
    }
}
