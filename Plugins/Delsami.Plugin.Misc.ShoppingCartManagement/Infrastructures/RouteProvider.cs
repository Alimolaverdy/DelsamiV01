using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShoppingCartManagement.Infrastructures
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => int.MaxValue;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "ShoppingCartManagement",
             pattern: $"Admin/ShoppingCart/CurrentCarts",
             defaults: new { controller = "DelsamiShoppingCartManagement", action = "CurrentCarts", area = "Admin" });
        }
    }
}
