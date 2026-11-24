using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.AmountRestriction;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Validator;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Factories;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Infrastructure.Permissions;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Factories;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IZarIranPaymentMethodModelFactory, ZarIranPaymentMethodModelFactory>();
            services.AddScoped<IZarIranStateProvinceService, ZarIranStateProvinceService>();
            services.AddScoped<IPaymentMethodRestrictionService, PaymentMethodRestrictionService>();
            services.AddScoped<IPermissionProvider, PaymentMethodStateProvinceRestrictionPermission>();
            services.AddScoped<ICheckoutModelFactory, ZarIranCheckoutModelFactory>();
            services.AddScoped<IPaymentMethodDiscountService, PaymentMethodDiscountService>();
            services.AddScoped<IPaymentService, ZarIranPaymentService>();
            services.AddScoped<IZarIranPaymentService, ZarIranPaymentService>();
            services.AddScoped<IPaymentMethodRewardPointService, PaymentMethodRewardPointService>();
            services.AddScoped<IPaymentMethodDiscountModelFactory, PaymentMethodDiscountModelFactory>();
            services.AddScoped<IPaymentMethodCustomerRoleRestrictionService, PaymentMethodCustomerRoleRestrictionService>();
            services.AddScoped<IPaymentMethodAmountRestrictionService, PaymentMethodAmountRestrictionService>();
            services.AddScoped<IPaymentMethodShippingRestrictionService, PaymentMethodShippingRestrictionService>();
            services.AddTransient<IValidator<PaymentMethodAmountRestrictionList>, AmountRestrictionValidator>();
        }
    }
}
