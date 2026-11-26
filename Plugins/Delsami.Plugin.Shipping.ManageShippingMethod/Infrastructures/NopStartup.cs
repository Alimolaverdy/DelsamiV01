using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Factories;
using Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Services;
using Delsami.Plugin.Shipping.ManageShippingMethod.Factories;
using Delsami.Plugin.Shipping.ManageShippingMethod.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Infrastructures
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
            services.AddScoped<IDelsamiShippingModelFactory, DelsamiShippingModelFactory>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IDelsamiStateProvinceService, DelsamiStateProvinceService>();
            services.AddScoped<IDelsamiShippingService, DelsamiShippingService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IBaseAdminModelFactory, BaseAdminModelFactory>();
            services.AddScoped<IShippingService, DelsamiManagingShippingService>();
            services.AddScoped<IFulfillmentDecisionService, FulfillmentDecisionService>();
            services.AddScoped<IFulfillmentDecisionFactory, FulfillmentDecisionFactory>();
        }
    }
}
