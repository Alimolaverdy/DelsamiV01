using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            AvailableSubStatuses = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();
            SelectedShippingMethods = new List<int>();
        }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedOrder.Configuration.OrderExpirationAsMinutes")]
        public int OrderExpirationAsMinutes { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedOrder.Configuration.OrderCompleteAsDays")]
        public int OrderCompleteAsDays { get; set; }

        public int SystemSubStatusId { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedOrder.Configuration.SelectedShippingMethods")]
        public IList<int> SelectedShippingMethods { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ExtendedOrder.Configuration.FulfillmentDecisionBaseUrl")]
        public string FulfillmentDecisionBaseUrl { get; set; }

        public IList<SelectListItem> AvailableShippingMethods { get; set; }

        public IList<SelectListItem> AvailableSubStatuses { get; set; }

    }
}
