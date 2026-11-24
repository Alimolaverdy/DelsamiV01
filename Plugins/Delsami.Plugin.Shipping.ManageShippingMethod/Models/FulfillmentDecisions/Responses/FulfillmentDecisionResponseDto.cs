using ZarIran.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Enums;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Responses
{
    public record FulfillmentDecisionResponseDto
    {
        public bool IsSuccessful { get; set; }

        public string Message { get; set; }

        public FulfillmentType Data { get; set; }
    }
}
