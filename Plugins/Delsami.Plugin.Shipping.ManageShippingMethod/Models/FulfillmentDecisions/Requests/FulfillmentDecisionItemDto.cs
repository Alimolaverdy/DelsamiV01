namespace Delsami.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Requests
{
    public record FulfillmentDecisionItemDto
    {
        public string Sku { get; set; }

        public int RequestedQuantity { get; set; }
    }
}
