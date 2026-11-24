using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Areas.Admin.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => int.MaxValue;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "CreateCategoryCustom",
                                                    pattern: $"Admin/Category/Create",
                                                    defaults: new { area = "Admin", controller = "CustomCategory", action = "Create" });

            endpointRouteBuilder.MapControllerRoute(name: "EditCategoryCustom",
                                        pattern: $"Admin/Category/Edit/{{id?}}",
                                        defaults: new { area = "Admin", controller = "CustomCategory", action = "Edit" });
        }
    }
}
