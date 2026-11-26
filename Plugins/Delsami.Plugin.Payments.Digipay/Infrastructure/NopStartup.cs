using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Payments.Digipay.Areas.Admin.Models.DigiPayRefund;
using Delsami.Plugin.Payments.Digipay.Areas.Admin.Validators;
using Delsami.Plugin.Payments.Digipay.Factories;
using Delsami.Plugin.Payments.Digipay.Services;
using Nop.Core.Infrastructure;

namespace Delsami.Plugin.Payments.Digipay.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDigiPayService, DigiPayService>();
            services.AddScoped<IBPGModelFactory, BPGModelFactory>();
			services.AddTransient<IValidator<DigiPayRefundModel>, DigiPayRefundModelValidator>();

		}
	}
}