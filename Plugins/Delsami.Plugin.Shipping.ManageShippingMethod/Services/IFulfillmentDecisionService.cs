using ZarIran.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Requests;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Responses;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Services
{
    public interface IFulfillmentDecisionService
    {
        Task<(bool IsSuccess, FulfillmentDecisionResponseDto Response)> GetWarehouseFulfillmentAsync(FulfillmentDecisionRequestDto request);
    }
}
