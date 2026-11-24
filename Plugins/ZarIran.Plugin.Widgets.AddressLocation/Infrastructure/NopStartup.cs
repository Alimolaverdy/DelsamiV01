using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Widgets.AddressLocation.Services;
using ZarIran.Plugin.Widgets.AddressLocation.Services.StateProvinceLocations;
using ZarIran.Plugin.Widgets.AddressLocation.Validators;
using Nop.Core.Infrastructure;
using Nop.Services.Common;

namespace ZarIran.Plugin.Widgets.AddressLocation.Infrastructure
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
            services.AddScoped<IStateProvinceLocationService, StateProvinceLocationService>();
            services.AddScoped<IALAddressAttributeService, ALAddressAttributeService>();
            services.AddScoped<IAddressAttributeFormatter, CustomAddressAttributeFormatter>();
            services.AddScoped<IAddressCityService, AddressCityService>();
            services.AddSingleton<IranBoundaryService>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {

                        //you can configure your custom policy
                        builder.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new AddressLocationViewExapnder());
            });

            services.AddValidatorsFromAssemblyContaining<ZarIranCustomerAddressEditValidator>();
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
