using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;

namespace Delsami.Plugin.Misc.CustomPdfGenerator.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "DownloadShipment",
                pattern: "Admin/Order/PdfPackagingSlip",
                defaults: new { controller = "CustomOrder", action = "PdfPackagingSlipPrint", area = "Admin" });
        }
    }
}
