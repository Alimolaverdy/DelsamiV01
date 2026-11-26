using Nop.Core.Configuration;

namespace Delsami.Plugin.Misc.ExtendedOrder;

public class ExtendedOrderSettings : ISettings
{
    public int OrderExpirationAsMinutes { get; set; }
    public int OrderCompleteAsDays { get; set; }
    public int systemSubStatusId { get; set; }
    public string SelectedShippingMethods { get; set; }
    public string FulfillmentDecisionBaseUrl { get; set; }
}
