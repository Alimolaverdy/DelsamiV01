using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.Core.Services;
using ZarIran.Plugin.Misc.ProductManagements.Areas.Admin.Factories;
using ZarIran.Plugin.Misc.ProductManagements.Factories;
using ZarIran.Plugin.Misc.ProductManagements.Services;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Localization;
using ZarIran.Plugin.Misc.ProductManagements.Data;
using Nop.Services.Shipping;
using Nop.Web.Factories;

namespace ZarIran.Plugin.Misc.ProductManagements.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ZarIranSlugRouteTranformer>();
            services.AddScoped<IWarehouseAddressCustomerDistanceService, WarehouseAddressCustomerDistanceService>();
            services.AddScoped<IWarehouseVendorService, WarehouseVendorService>();
            services.AddScoped<IWarehouseVendorFactory, WarehouseVendorFactory>();
            services.AddScoped<IOrderProcessingService, ZarIranOrderProcessingService>();
            services.AddScoped<IVirtualProductMappingService, VirtualProductMappingService>();
            services.AddScoped<IShoppingCartService, ZarIranShoppingCartService>();
            services.AddScoped<IProductModelFactory, Factories.ZarIranProductModelFactory>();
            services.AddScoped<IZarIranProductModelFactory, Areas.Admin.Factories.ZarIranProductModelFactory>();
            services.AddScoped<Nop.Web.Areas.Admin.Factories.IProductModelFactory, Areas.Admin.Factories.ZarIranProductModelFactory>();
            services.AddScoped<IProductService, ZarIranProductService>();
            services.AddScoped<ICustomZarIranProductService, CustomZarIranProductService>();
            services.AddScoped<IGeneratedVirtualProductService, GeneratedVirtualProductService>();
            services.AddScoped<IReservedStockQuantityService, ReservedStockQuantityService>();
            services.AddScoped<IReservedStockQuantityHistoryService, ReservedStockQuantityHistoryService>();
            services.AddScoped<IShoppingCartModelFactory, ZarIranShoppingCartModelFactory>();
            services.AddScoped<IGroupedProductStockService, GroupedProductStockService>();
            services.AddScoped<IZarIranBackInStockSubscriptionService, ZarIranBackInStockSubscriptionService>();
            services.AddScoped<IOtherBrandProductService, OtherBrandProductService>();
            services.AddScoped<IBundleProductService, BundleProductService>();
            services.AddScoped<IProductPricingService, ProductPricingService>();
            services.AddScoped<IZarIranPriceCalculationService, ZarIranPriceCalculationService>();

            services.AddScoped<IBundleProductService, BundleProductService>();

            services.AddScoped<IRecentlyViewedProductsService, ZarIranRecentlyViewedProductsService>();



            services.Decorate<IShippingService, ZarIranShippingService>();

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
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            // Code-only deployments do not necessarily queue a plugin upgrade.
            using var scope = application.ApplicationServices.CreateScope();
            ProductPageLocaleMigration.EnsureResources(
                scope.ServiceProvider.GetRequiredService<ILanguageService>(),
                scope.ServiceProvider.GetRequiredService<ILocalizationService>());
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue - 1;
    }
}
