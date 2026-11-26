using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Shipping.Tipax.Factories;
using Delsami.Plugin.Shipping.Tipax.Services;
using Nop.Core.Infrastructure;
using System;

namespace Delsami.Plugin.Shipping.Tipax.Infrastucture
{
    public class NopStartup : INopStartup
    {
        public NopStartup()
        {
        }

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITipaxService, TipaxService>();
            services.AddScoped<ITipaxApiService, TipaxApiService>();
            services.AddScoped<IDelsamiTipaxCityMappingService, DelsamiTiapxCityMappingService>();
            services.AddScoped<ITipaxModelsFactory, TipaxModelsFactory>();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.UseCors();

        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 3000;
    }
}
