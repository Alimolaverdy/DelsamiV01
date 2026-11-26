using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Delsami.Plugin.Misc.CategoryEnhancer.Areas.Admin.Factroies;
using Delsami.Plugin.Misc.CategoryEnhancer.Services;
using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Factories;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICategoryIconService, CategoryIconService>();
            services.AddScoped<ICategoryModelFactory, DelsamiCategoryModelFactory>();
            services.AddScoped<ICanonicalCategoryService, CanonicalCategoryService>();
            services.AddScoped<IDelsamiUrlRecordService, DelsamiUrlRecordService>();
            services.AddScoped<ICategorySpecificationAttributeService, CategorySpecificationAttributeService>();
        }

    }
}
