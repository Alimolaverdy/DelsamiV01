using Nop.Core.Configuration;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.ManageShippingMethod
{
    public class ManageShippingMethodSetting : ISettings
    {
        public int CountryId { get; set; }

        public string DisabledShippingMethods { get; set; }

        public bool FulfillmentDecisionEnabled { get; set; }

        public string FulfillmentDecisionBaseDomain { get; set; }
    }
}
