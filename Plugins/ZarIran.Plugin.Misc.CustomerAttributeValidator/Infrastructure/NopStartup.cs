using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Factories;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Validators;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Domain;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Helpers;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Validators.Customer;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Infrastructure
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
            services.AddScoped<ICustomerAttributeValidationFactory, CustomerAttributeValidationFactory>();
            services.AddScoped<ICustomerAttributeParser, CustomCustomerAttributeParser>();
            services.AddScoped<ICustomerAttributeValidationService, CustomerAttributeValidationService>();
            services.AddScoped<IOrderProcessingService, CustomOrderProcessingService>();
            services.AddScoped<ICustomCustomerAttributesXMLService, CustomCustomerAttributesXMLService>();
            services.AddScoped<ICustomerProfileValidationService, CustomerProfileValidationService>();
            //services.AddScoped<IShoppingCartService, CustomShoppingCartService>();
            services.AddTransient<IValidator<CustomerAttributeValidationModel>, CustomerAttributeValidationValidator>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomerAttributeValidatorViewLocationExpander());
            });
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
        public int Order => 3000;
    }
}
