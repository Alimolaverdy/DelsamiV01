using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Misc.ShoppingCartManagement.Areas.Admin.Controllers;
using Delsami.Plugin.Misc.ShoppingCartManagement.Areas.Admin.Factories;
using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using System;

namespace Delsami.Plugin.Misc.ShoppingCartManagement.Infrastructures
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IShoppingCartModelFactory, DelsamiShoppingCartModelFactory>();
        }
    }
}
