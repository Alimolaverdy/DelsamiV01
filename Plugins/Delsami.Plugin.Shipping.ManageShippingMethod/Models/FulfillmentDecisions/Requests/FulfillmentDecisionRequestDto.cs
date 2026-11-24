using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Requests
{
    public record FulfillmentDecisionRequestDto
    {
        public string ShippingProvince { get; set; }
        public string ShippingCity { get; set; }
        public List<FulfillmentDecisionItemDto> Items { get; set; } =new();
    }
}
