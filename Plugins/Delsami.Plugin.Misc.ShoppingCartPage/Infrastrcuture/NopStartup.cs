using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.ShoppingCartPage.Factories;
using ZarIran.Plugin.Misc.ShoppingCartPage.Infrastrcuture;
using Nop.Core.Infrastructure;
using Nop.Web.Factories;

namespace ZarIran.Plugin.Misc.CarInfo.Infrastrcuture
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
            services.AddScoped<ZarIranShoppingCartModelFactory>();

            services.AddScoped<IZarIranShoppingCartModelFactory>(sp =>
                sp.GetRequiredService<ZarIranShoppingCartModelFactory>());

            services.AddScoped<IShoppingCartModelFactory>(sp =>
                sp.GetRequiredService<ZarIranShoppingCartModelFactory>());

            services.AddScoped<IZarIranShoppingCartModelFactory, ZarIranShoppingCartModelFactory>();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ShppingCartPageViewLocationExpander());
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
        public int Order => int.MaxValue;
    }
}
