using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Misc.TorobApi.Filters;
using Delsami.Plugin.Misc.TorobApi.Middlewares;
using Delsami.Plugin.Misc.TorobApi.Services;
using Delsami.Plugin.Misc.TorobApi.Validators;
using Nop.Core.Infrastructure;

namespace Delsami.Plugin.Misc.TorobApi.Infrastructure
{
    internal class NopStartup : INopStartup
    {
        public int Order => 1;

        public void Configure(IApplicationBuilder application)
        {
            application.UseMiddleware<SaveTorobClIdInCookieMiddleware>();
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ITokenValidator, TokenValidator>();
            services.AddScoped<ITorobProductService, TorobProductService>();
            services.AddScoped<IpFilterAttribute>();
            services.AddScoped<TorobAuthenticationFilter>();

        }
    }
}
