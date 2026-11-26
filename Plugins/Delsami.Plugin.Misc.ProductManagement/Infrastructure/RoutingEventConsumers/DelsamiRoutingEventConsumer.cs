using Nop.Services.Events;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Mvc.Routing;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Infrastructure.RoutingEventConsumers
{
    public class DelsamiRoutingEventConsumer : IConsumer<GenericRoutingEvent>
    {
        public Task HandleEventAsync(GenericRoutingEvent eventMessage)
        {
            var values = eventMessage.RouteValues;
            var urlRecord = eventMessage.UrlRecord;

            if (urlRecord is not null && urlRecord.EntityName == "Product")
            {
                values[NopRoutingDefaults.RouteValue.Controller] = "CustomProduct";
                values[NopRoutingDefaults.RouteValue.Action] = "ProductDetails";
                values[NopRoutingDefaults.RouteValue.ProductId] = urlRecord.EntityId;
                values[NopRoutingDefaults.RouteValue.SeName] = urlRecord.Slug;

                eventMessage.Handled = true;
            }
            return Task.CompletedTask;
        }
    }
}
