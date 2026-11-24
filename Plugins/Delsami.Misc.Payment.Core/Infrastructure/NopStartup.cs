using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Payment.Core.Areas.Admin.Factories;
using ZarIran.Plugin.Payment.Core.Infrastructure.Permissions;
using ZarIran.Plugin.Payment.Core.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Security;

namespace ZarIran.Plugin.Payment.Core.Infrastructure
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
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentModelFactory, PaymentModelFactory>();
            services.AddScoped<IPermissionProvider, PaymentCorePermission>();
            services.AddScoped<IPaymentApiCaller, PaymentApiCaller>();
            services.AddScoped<IPaymentResilienceService, PaymentResilienceService>();
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
