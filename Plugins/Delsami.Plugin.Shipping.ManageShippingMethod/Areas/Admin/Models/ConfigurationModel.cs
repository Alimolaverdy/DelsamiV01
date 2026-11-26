using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Areas.Admin.Models
{
    public class ConfigurationModel
    {
        public ConfigurationModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();
        }


        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Country")]
        public int CountryId { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ManageShippingMethod.DisabledShippingMethods")]
        public IList<int>DisabledShippingMethods{ get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ManageShippingMethod.FulfillmentDecisionEnabled")]
        public bool FulfillmentDecisionEnabled { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ManageShippingMethod.FulfillmentDecisionBaseDomain")]
        public string FulfillmentDecisionBaseDomain { get; set; }

        public IList<SelectListItem> AvailableShippingMethods{ get; set; }
    }
}