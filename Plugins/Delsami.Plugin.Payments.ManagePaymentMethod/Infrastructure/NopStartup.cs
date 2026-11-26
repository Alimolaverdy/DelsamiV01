using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.AmountRestriction;
using Delsami.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Validator;
using Delsami.Plugin.Payments.ManagePaymentMethod.Factories;
using Delsami.Plugin.Payments.ManagePaymentMethod.Infrastructure.Permissions;
using Delsami.Plugin.Payments.ManagePaymentMethod.Services;
using Nop.Core.Infrastructure;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Factories;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDelsamiPaymentMethodModelFactory, DelsamiPaymentMethodModelFactory>();
            services.AddScoped<IDelsamiStateProvinceService, DelsamiStateProvinceService>();
            services.AddScoped<IPaymentMethodRestrictionService, PaymentMethodRestrictionService>();
            services.AddScoped<IPermissionProvider, PaymentMethodStateProvinceRestrictionPermission>();
            services.AddScoped<ICheckoutModelFactory, DelsamiCheckoutModelFactory>();
            services.AddScoped<IPaymentMethodDiscountService, PaymentMethodDiscountService>();
            services.AddScoped<IPaymentService, DelsamiPaymentService>();
            services.AddScoped<IDelsamiPaymentService, DelsamiPaymentService>();
            services.AddScoped<IPaymentMethodRewardPointService, PaymentMethodRewardPointService>();
            services.AddScoped<IPaymentMethodDiscountModelFactory, PaymentMethodDiscountModelFactory>();
            services.AddScoped<IPaymentMethodCustomerRoleRestrictionService, PaymentMethodCustomerRoleRestrictionService>();
            services.AddScoped<IPaymentMethodAmountRestrictionService, PaymentMethodAmountRestrictionService>();
            services.AddScoped<IPaymentMethodShippingRestrictionService, PaymentMethodShippingRestrictionService>();
            services.AddTransient<IValidator<PaymentMethodAmountRestrictionList>, AmountRestrictionValidator>();
        }
    }
}
