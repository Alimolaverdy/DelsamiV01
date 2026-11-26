using Delsami.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Requests;
using Delsami.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Responses;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Services
{
    public interface IFulfillmentDecisionService
    {
        Task<(bool IsSuccess, FulfillmentDecisionResponseDto Response)> GetWarehouseFulfillmentAsync(FulfillmentDecisionRequestDto request);
    }
}
