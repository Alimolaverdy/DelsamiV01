using Newtonsoft.Json;
using Delsami.Plugin.Misc.Core.Services.ApiCaller;
using Delsami.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Requests;
using Delsami.Plugin.Shipping.ManageShippingMethod.Models.FulfillmentDecisions.Responses;
using Nop.Services.Logging;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Services
{
    public class FulfillmentDecisionService : IFulfillmentDecisionService
    {
        private readonly ManageShippingMethodSetting _manageShippingMethodSetting;
        private readonly IApiCaller _apiCaller;
        private readonly ILogger _logger;

        public FulfillmentDecisionService(ManageShippingMethodSetting manageShippingMethodSetting,
                                          IApiCaller apiCaller,
                                          ILogger logger)
        {
            _manageShippingMethodSetting = manageShippingMethodSetting;
            _apiCaller = apiCaller;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, FulfillmentDecisionResponseDto Response)> GetWarehouseFulfillmentAsync(FulfillmentDecisionRequestDto request)
        {
            if (request is null)
                return (false, null);

            var endPointAddress = "/Fulfillment/GetFulfillmentType";

            var url = _manageShippingMethodSetting.FulfillmentDecisionBaseDomain.TrimEnd('/') + endPointAddress;
            try
            {
                var response = await _apiCaller.PostAsync<FulfillmentDecisionRequestDto, FulfillmentDecisionResponseDto>(url, request);

                if (!response.IsSuccess)
                {
                    await _logger.ErrorAsync($"Fulfillment Decision was unsuccessfuly due to: {JsonConvert.SerializeObject(response)}");
                    return (false, null);
                }

                return (true, response.Data);
            }

            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
                return (false, null);
            }
        }
    }
}
