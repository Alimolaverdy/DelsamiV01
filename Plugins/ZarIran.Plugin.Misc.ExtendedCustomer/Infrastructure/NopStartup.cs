using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.Core.Bases;
using ZarIran.Plugin.Misc.ExtendedCustomer.Factories.CustomerShebaNumbers;
using ZarIran.Plugin.Misc.ExtendedCustomer.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using Nop.Services.Customers.Caching;
using Nop.Services.Security;
using System.Linq;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MvcOptions>(options => options.Conventions.Add(new CustomerInfoConvention()));
            services.AddScoped<ICustomerShebaNumberService, CustomerShebaNumberService>();
            services.AddScoped<ICustomerNavigationItems, CustomerNavigationItems>();
            services.AddScoped<ICustomerShebaNumberModelFactory, CustomerShebaNumberModelFactory>();
            services.AddScoped<IPermissionService, ZarIranPermissionService>();
            var originalRoleMappingConsumers = services
            .Where(descriptor => descriptor.ImplementationType ==
                typeof(CustomerCustomerRoleMappingCacheEventConsumer))
            .ToArray();

                    foreach (var descriptor in originalRoleMappingConsumers)
                    {
                        services.Remove(descriptor);
                    }
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => int.MaxValue;
    }
}