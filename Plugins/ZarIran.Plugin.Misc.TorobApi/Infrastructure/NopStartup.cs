using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.TorobApi.Filters;
using ZarIran.Plugin.Misc.TorobApi.Middlewares;
using ZarIran.Plugin.Misc.TorobApi.Services;
using ZarIran.Plugin.Misc.TorobApi.Validators;
using Nop.Core.Infrastructure;

namespace ZarIran.Plugin.Misc.TorobApi.Infrastructure
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
