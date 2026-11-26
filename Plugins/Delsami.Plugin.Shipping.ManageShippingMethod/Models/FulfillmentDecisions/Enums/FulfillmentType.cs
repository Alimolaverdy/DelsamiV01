using System.ComponentModel;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Enums
{
    public enum FulfillmentType
    {
        [Description("تک انباری")]
        SingleWarehouse = 1,

        [Description("چند انباری")]
        MultiWarehouse = 2
    }
}
