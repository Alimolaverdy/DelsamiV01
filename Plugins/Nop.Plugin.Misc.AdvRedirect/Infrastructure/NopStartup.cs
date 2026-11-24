using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.AdvRedirect.Services;

namespace Nop.Plugin.Misc.AdvRedirect.Infrastructure
{

    public class NopStartup : INopStartup
    {
        public int Order => 100;

        public void Configure(IApplicationBuilder application)
        {
            //application.UseCustomRedirect();
        }

        
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRedirectionsService, RedirectionsService>();
			services.AddScoped<IDataIOService,DataIOService>();
			services.AddScoped<IAdvRedirectHistoryService, AdvRedirectHistoryService>();
		}
    }
}