using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Factories;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Factories;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Infrastructures
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue - 1;

        public void Configure(IApplicationBuilder application)
        {

        }
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IZarIranShippingModelFactory, ZarIranShippingModelFactory>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IZarIranStateProvinceService, ZarIranStateProvinceService>();
            services.AddScoped<IZarIranShippingService, ZarIranShippingService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IBaseAdminModelFactory, BaseAdminModelFactory>();
            services.AddScoped<IShippingService, ZarIranManagingShippingService>();
            services.AddScoped<IFulfillmentDecisionService, FulfillmentDecisionService>();
            services.AddScoped<IFulfillmentDecisionFactory, FulfillmentDecisionFactory>();
        }
    }
}
