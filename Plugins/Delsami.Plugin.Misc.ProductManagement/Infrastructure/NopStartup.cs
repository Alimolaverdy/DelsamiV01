using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Misc.ProductManagement.Areas.Admin.Factories;
using Delsami.Plugin.Misc.ProductManagement.Factories;
using Delsami.Plugin.Misc.ProductManagement.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Web.Factories;

namespace Delsami.Plugin.Misc.ProductManagement.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DelsamiSlugRouteTranformer>();
            services.AddScoped<IWarehouseAddressCustomerDistanceService, WarehouseAddressCustomerDistanceService>();
            services.AddScoped<IWarehouseVendorService, WarehouseVendorService>();
            services.AddScoped<IWarehouseVendorFactory, WarehouseVendorFactory>();
            services.AddScoped<IOrderProcessingService, DelsamiOrderProcessingService>();
            services.AddScoped<IVirtualProductMappingService, VirtualProductMappingService>();
            services.AddScoped<IShoppingCartService, DelsamiShoppingCartService>();
            services.AddScoped<IProductModelFactory, Factories.DelsamiProductModelFactory>();
            services.AddScoped<IDelsamiProductModelFactory, Areas.Admin.Factories.DelsamiProductModelFactory>();
            services.AddScoped<Nop.Web.Areas.Admin.Factories.IProductModelFactory, Areas.Admin.Factories.DelsamiProductModelFactory>();
            services.AddScoped<IProductService, DelsamiProductService>();
            services.AddScoped<ICustomDelsamiProductService, CustomDelsamiProductService>();
            services.AddScoped<IGeneratedVirtualProductService, GeneratedVirtualProductService>();
            services.AddScoped<IReservedStockQuantityService, ReservedStockQuantityService>();
            services.AddScoped<IReservedStockQuantityHistoryService, ReservedStockQuantityHistoryService>();
            services.AddScoped<IShoppingCartModelFactory, DelsamiShoppingCartModelFactory>();
            services.AddScoped<IGroupedProductStockService, GroupedProductStockService>();
            services.AddScoped<IBundleProductService, BundleProductService>();



            services.Decorate<IShippingService, DelsamiShippingService>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ExtendedVendorViewExapnder());
            });
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //application.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "Delsami.Plugin.Misc.ExtendedVendor",
            //        pattern: "customproduct",
            //        defaults: new { controller = "CustomProduct", action = "Index" }
            //    );
            //});
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue;
    }
}
