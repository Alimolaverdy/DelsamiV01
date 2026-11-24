using Microsoft.Extensions.Configuration;
using ZarIran.Plugin.Misc.TorobApi.Models.Validators;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.TorobApi.Validators
{
    public class TokenValidator : ITokenValidator
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly TorobApiSettings _torobApiSettings;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public TokenValidator(HttpClient httpClient,
                              IConfiguration configuration,
                              TorobApiSettings torobApiSettings,
                              IStoreContext storeContext)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _torobApiSettings = torobApiSettings;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task<TokenValidationResult> ValidateToken(string token)
        {
            try
            {
                var validationUrl = "https://extractor.torob.com/validate_token/";
                var shopDomain = _storeContext.GetCurrentStore().Url;

                var request = new HttpRequestMessage(HttpMethod.Post, validationUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", token),
                    new KeyValuePair<string, string>("shop_domain", shopDomain),
                    new KeyValuePair<string, string>("version", "1.0.0")
                });

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TokenValidationResponse>(responseContent);

                return new TokenValidationResult
                {
                    IsValid = result?.Success == true && result.Message == "the token is valid",
                    Error = result?.Error
                };
            }
            catch (Exception ex)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = ex.Message
                };
            }
        }

        #endregion
    }
}
