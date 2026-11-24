using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nop.Web.Controllers;
using ZarIran.Plugin.Misc.ExtendedCustomer.Controllers;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Infrastructure
{
    public sealed class CustomerInfoConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            var original = application.Controllers.SingleOrDefault(controller =>
                controller.ControllerType.AsType() == typeof(CustomerController));
            var replacement = application.Controllers.SingleOrDefault(controller =>
                controller.ControllerType.AsType() == typeof(ExtendedCustomerInfoController));

            if (original == null || replacement == null)
                return;

            // Keep the original route and replace only the two Info actions.
            replacement.ControllerName = original.ControllerName;
            foreach (var action in replacement.Actions.ToArray())
            {
                if (action.ActionMethod.DeclaringType != typeof(ExtendedCustomerInfoController))
                    replacement.Actions.Remove(action);
            }

            foreach (var action in original.Actions.Where(action => action.ActionName == nameof(CustomerController.Info)).ToArray())
                original.Actions.Remove(action);
        }
    }
}
