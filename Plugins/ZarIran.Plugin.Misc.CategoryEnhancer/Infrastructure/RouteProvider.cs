using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 1;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "CategoryEnhancer",
                    pattern: "Admin/Category/Create",
                    defaults: new { controller = "ZarIranCategoryEnhancer", action = "Create", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "EditCategoryEnhancer",
                    pattern: "Admin/Category/Edit/{id?}",
                    defaults: new { controller = "ZarIranCategoryEnhancer", action = "Edit", area = "Admin" });
        }
    }
}
