using Microsoft.Extensions.Caching.Memory;
using ZarIran.Plugin.Misc.Core.Services.ApiCaller;
using ZarIran.Plugin.Payment.Core.Services;
using ZarIran.Plugin.Payments.TorobPay.Services.Dtos;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Payments.TorobPay;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static ZarIran.Plugin.Payments.TorobPay.Services.Dtos.PaymentDto;

namespace ZarIran.Plugin.Payments.TorobPay.Services
{
    public class TorobPayApiService : ITorobPayApiService
    {
        #region Fields
        private readonly TorobPaySettings _settings;
        private readonly ILogger _logger;
        private readonly IPaymentApiCaller _paymentApiCaller;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "TorobPay.AccessToken";

        #endregion

        #region Ctor
        public TorobPayApiService(TorobPaySettings settings,
                                  IPaymentApiCaller paymentApiCaller,
                                  ILogger logger,
                                  IMemoryCache cache)
        {
            _settings = settings;
            _logger = logger;
            _paymentApiCaller = paymentApiCaller;
            _cache = cache;
        }
        #endregion

        #region Methods

        public async Task<EligibleResponse> CheckEligibilityAsync(decimal amount)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                var amountStr = amount.ToString("0.##", CultureInfo.InvariantCulture);
                var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.EligibleEndpoint}?amount={amountStr}";

                var result = await _paymentApiCaller.GetAsync<EligibleDto>(url,bearerToken: token);

                if (!result.IsSuccess || result.Data == null)
                    throw new Exception("Failed to check eligibility on TorobPay.");

                return result.Data.response;
            }
            catch (TaskCanceledException ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, $"CheckEligibilityAsync Of TorobPay Timeout After {_settings.TimeoutSeconds} Seconds",ex.Message);
                throw new TimeoutException($"CheckEligibilityAsync timed out after {_settings.TimeoutSeconds} seconds.", ex);
            }
        }



        public async Task<PaymentTokenResponse> CreatePaymentTokenAsync(PaymentTokenRequest request)
        {
            var token = await GetAccessTokenAsync();

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.PaymentTokenEndpoint}";

            var result = await _paymentApiCaller.PostAsync<PaymentTokenRequest, PaymentTokenResponseRoot>(
                url,
                request,
                bearerToken: token);

            if (!result.IsSuccess || result.Data == null)
                throw new Exception("Failed to create payment token on TorobPay.");

            return result.Data.response;
        }
        public async Task<VerifyResponse> VerifyAsync(string paymentToken)
        {
            var token = await GetAccessTokenAsync();

            var verifyRequest = new VerifyRequest { paymentToken = paymentToken };

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.VerifyEndpoint}";

            var result = await _paymentApiCaller.PostAsync<VerifyRequest, VerifyResponseRoot>(
                url,
                verifyRequest,
                bearerToken: token);

            if (!result.IsSuccess || result.Data == null)
                throw new Exception("Failed to verify payment on TorobPay.");

            return result.Data.response;
        }

        public async Task<SettleResponse> SettleAsync(string paymentToken)
        {
            var token = await GetAccessTokenAsync();

            var settleRequest = new SettleRequest { paymentToken = paymentToken };

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.SettleEndpoint}";

            var result = await _paymentApiCaller.PostAsync<SettleRequest, SettleResponseRoot>(
                url,
                settleRequest,
                bearerToken: token);

            if (!result.IsSuccess || result.Data == null)
                throw new Exception("Failed to settle payment on TorobPay.");

            return result.Data.response;
        }

        public async Task CancelAsync(string paymentToken)
        {
            var token = await GetAccessTokenAsync();

            var cancelRequest = new CancelRequest { paymentToken = paymentToken };

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.CancelEndpoint}";

            var result = await _paymentApiCaller.PostAsync<CancelRequest, object>(
                url,
                cancelRequest,
                bearerToken: token);

            if (!result.IsSuccess)
                throw new Exception("Failed to cancel payment on TorobPay.");
        }

        public async Task RevertAsync(string paymentToken)
        {
            var token = await GetAccessTokenAsync();

            var revertRequest = new { paymentToken = paymentToken };

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.RevertEndpoint}";

            var result = await _paymentApiCaller.PostAsync<object, object>(
                url,
                revertRequest,
                bearerToken: token);

            if (!result.IsSuccess)
                throw new Exception("Failed to revert payment on TorobPay.");
        }
        public async Task<StatusResponse> GetStatusAsync(string paymentToken)
        {
            var token = await GetAccessTokenAsync();

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.StatusEndpoint}?paymentToken={paymentToken}";

            var result = await _paymentApiCaller.GetAsync<StatusResponseRoot>(
                url,
                bearerToken: token);

            if (!result.IsSuccess || result.Data == null)
                throw new Exception("Failed to get status from TorobPay.");

            return result.Data.response;
        }
        public async Task UpdateAsync(UpdateRequest request)
        {
            var token = await GetAccessTokenAsync();

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.UpdateEndpoint}";

            var result = await _paymentApiCaller.PostAsync<UpdateRequest, object>(
                url,
                request,
                bearerToken: token);

            if (!result.IsSuccess)
                throw new Exception("Failed to update payment on TorobPay.");
        }


        private async Task<string> GetAccessTokenAsync()
        {
            if (_cache.TryGetValue(CacheKey, out string token))
                return token;

            var body = new TokenDto(_settings.Username, _settings.Password);

            var url = $"{_settings.BaseUrl}{TorobPayPaymentDefaults.OAuthTokenEndpoint}";

            var result = await _paymentApiCaller.PostAsync<TokenDto, OAuthTokenResponse>(
                url,
                body,
                headers: new Dictionary<string, string>
                {
            { "Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}")) }
                });

            if (!result.IsSuccess || result.Data == null)
                throw new Exception("Failed to get access token from TorobPay.");

            token = result.Data.access_token;
            _cache.Set(CacheKey, token, TimeSpan.FromMinutes(_settings.TokenCacheMinutes));

            return token;
        }
        #endregion
    }
}
