using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZarIran.Plugin.Misc.CategoryEnhancer.Areas.Admin.Factroies;
using ZarIran.Plugin.Misc.CategoryEnhancer.Services;
using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Factories;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Infrastructure
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
            services.AddScoped<ICategoryModelFactory, ZarIranCategoryModelFactory>();
            services.AddScoped<ICanonicalCategoryService, CanonicalCategoryService>();
            services.AddScoped<IZarIranUrlRecordService, ZarIranUrlRecordService>();
            services.AddScoped<ICategorySpecificationAttributeService, CategorySpecificationAttributeService>();
        }

    }
}
