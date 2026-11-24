using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 1;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = string.Empty;

            if (DataSettingsManager.IsDatabaseInstalled())
            {
                var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //this pattern is set once at the application start, when we don't have the selected language yet
                    //so we use 'en' by default for the language value, later it'll be replaced with the working language code
                    var code = "en";
                    lang = $"{{{NopRoutingDefaults.RouteValue.Language}:maxlength(2):{NopRoutingDefaults.LanguageParameterTransformer}={code}}}";
                }
            }

            endpointRouteBuilder.MapControllerRoute(name: "ZarIranOrderDetails",
                pattern: $"{lang}/orderdetails/{{orderId:min(0)}}",
                defaults: new { controller = "ZarIranShipmentManagement", action = "Details" });

            endpointRouteBuilder.MapControllerRoute(name: "ZarIranShipmentManagement",
                pattern: $"Admin/Order/ShipmentDetails/{{id?}}",
                defaults: new { controller = "ShipmentManagementOrder", action = "ShipmentDetails", area = "Admin" });


            endpointRouteBuilder.MapControllerRoute(name: "CustomerOrders",
                pattern: $"{lang}/order/history",
                defaults: new { controller = "ZarIranShipmentManagement", action = "CustomerOrders" });

            //endpointRouteBuilder.MapControllerRoute(name: "ZarIranShipmentManagement",
            //    pattern: $"Area/Admin/Order/ShipmentDetails",
            //    defaults: new { controller = "ShipmentManagementOrder", action = "ShipmentDetails", area = "Admin" });
        }
    }
}
