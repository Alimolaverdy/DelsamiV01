using Delsami.Plugin.Misc.ExtendedOrder.Model.WarehouseLocations;
using Nop.Services.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Services.ApiCaller;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public class WarehouseLocationService : IWarehouseLocationService
    {
        #region Fields

        private readonly IApiCaller _apiCaller;
        private readonly ExtendedOrderSettings _extendedOrderSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public WarehouseLocationService(IApiCaller apiCaller, ExtendedOrderSettings extendedOrderSettings, ILogger logger)
        {
            _apiCaller = apiCaller;
            _extendedOrderSettings = extendedOrderSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<WarehouseLocationResponseModel> GetOrderWarehouseFromFulfillmentAsync(WarehouseLocationRequestModel orderRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_extendedOrderSettings.FulfillmentDecisionBaseUrl))
                    return null;

                var url = $"{_extendedOrderSettings.FulfillmentDecisionBaseUrl.TrimEnd('/')}/api/Warehouse/GetOrderWarehouse";
                var response = await _apiCaller.PostAsync<WarehouseLocationRequestModel, WarehouseLocationResponseModel>(url, orderRequest);

                if (!response.IsSuccess)
                    return null;

                return response.Data;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);

                return null;
            }
        }

        #endregion
    }
}