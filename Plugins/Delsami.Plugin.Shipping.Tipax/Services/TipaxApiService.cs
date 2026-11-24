using Azure;
using Newtonsoft.Json;
using ZarIran.Plugin.Shipping.Tipax.Models.TipaxApi;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Tipax.Services
{
    public class TipaxApiService : ITipaxApiService
    {
        #region Fields

        private readonly TipaxSettings _tipaxSettings;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public TipaxApiService(TipaxSettings tipaxSettings,
                               ILogger logger,
                               HttpClient httpClient)
        {
            _tipaxSettings = tipaxSettings;
            _logger = logger;
            _httpClient = httpClient;
            if (!string.IsNullOrWhiteSpace(_tipaxSettings.BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_tipaxSettings.BaseUrl);
            }
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        #endregion

        #region Methods

        public async Task<List<PricingResultModel>> GetPricingAsync(PricingRequestModel model, string accessToken)
        {
            var contentStr = System.Text.Json.JsonSerializer.Serialize(model);

            var content = new StringContent(contentStr,Encoding.UTF8,"application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                HttpResponseMessage response;

                if (_tipaxSettings.TimeoutSeconds > 0)
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_tipaxSettings.TimeoutSeconds));
                    response = await _httpClient.PostAsync("pricing", content, cts.Token);
                }
                else
                {
                    response = await _httpClient.PostAsync("pricing", content);
                }

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PricingResultModel>>(result);
            }
            catch (TaskCanceledException ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error,$"SendPricingRequestAsync Timeout After {_tipaxSettings.TimeoutSeconds} Seconds", ex.Message);
                throw new TimeoutException();
            }
        }

        public async Task<TokenResponseModel> GetTokenAsync(TokenRequestModel tokenRequest)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(tokenRequest),
                                            Encoding.UTF8,
                                            "application/json");

            var response = await _httpClient.PostAsync("account/token", content);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TokenResponseModel>(result);
        }

        #endregion
    }
}
