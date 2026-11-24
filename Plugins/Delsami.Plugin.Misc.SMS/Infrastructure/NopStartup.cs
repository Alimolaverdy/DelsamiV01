using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.SMS.Infrastructure.ExternalServices;
using ZarIran.Plugin.Misc.SMS.Servicecs;

namespace ZarIran.Plugin.Misc.SMS.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 10000;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISMSSender, SMSSender>();
            services.AddScoped<IKavehNegarSMSSenderService, KavehNegarSMSSenderService>();
        }
    }
}
