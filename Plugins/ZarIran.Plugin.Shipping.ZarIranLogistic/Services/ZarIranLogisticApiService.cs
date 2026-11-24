using ZarIran.Plugin.Shipping.ZarIranLogistic.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Services
{
    public class ZarIranLogisticApiService : IZarIranLogisticApiService
    {
        #region Fields

        private readonly ZarIranLogisticSettings _settings;

        #endregion

        #region Ctor

        public ZarIranLogisticApiService(ZarIranLogisticSettings settings)
        {
            _settings = settings;
        }

        #endregion

        #region  Methods

        public async Task<CloseCargoAndOrderBySellSiteResultModel> CloseCargoAndOrderBySellSiteAsync(int shipmentId)
        {
            try
            {
                using (var clinet = new HttpClient())
                {
                    var payload = new { shipmentId, token = _settings.ApiToken };

                    var url = $"{_settings.ApiBaseUrl}/couriers/closecargoandorderbysellsite";

                    var response = await clinet.PostAsJsonAsync(url, payload);

                    string result = await response.Content.ReadAsStringAsync();

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<CloseCargoAndOrderBySellSiteResultModel>(result);
                }
            }
            catch
            {
                return new CloseCargoAndOrderBySellSiteResultModel
                {
                    message = "خطای اتصال به لجستیک",
                    value = false,
                    statusCode = 0
                };
            }
        }

        #endregion
    }
}
