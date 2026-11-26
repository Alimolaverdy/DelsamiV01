//using Nop.Plugin.Payments.TorobPay;
//using System;
//using System.Net.Http.Headers;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Caching.Memory;
//using Delsami.Plugin.Payments.TorobPay.Services.Dtos;


//namespace Delsami.Plugin.Payments.TorobPay.Services
//{
//    public class TorobPayAuthService : ITorobPayAuthService
//    {
//        #region Fields
//        private readonly TorobPaySettings _settings;
//        private readonly IMemoryCache _cache;
//        private readonly IHttpClientFactory _httpClientFactory;
//        private const string CacheKey = "TorobPay.AccessToken";
//        #endregion

//        #region Ctor
//        public TorobPayAuthService(TorobPaySettings settings,
//            IMemoryCache cache,
//            IHttpClientFactory httpClientFactory)
//        {
//            _settings = settings;
//            _cache = cache;
//            _httpClientFactory = httpClientFactory;
//        }
//        #endregion

//        #region Methods
//        public async Task<string> GetAccessTokenAsync()
//        {
//            if (_cache.TryGetValue(CacheKey, out string token))
//                return token;

//            var client = _httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri(_settings.BaseUrl);

//            var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);

//            var body = new TokenDto(_settings.Username, _settings.Password);
//            var json = JsonSerializer.Serialize(body);
//            var response = await client.PostAsync(TorobPayPaymentDefaults.OAuthTokenEndpoint, new StringContent (json, Encoding.UTF8, "application/json"));
//            response.EnsureSuccessStatusCode();

//            var resJson = await response.Content.ReadAsStringAsync();
//            var res = JsonSerializer.Deserialize<OAuthTokenResponse>(resJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//            token = res.access_token;
//            _cache.Set(CacheKey, token, TimeSpan.FromMinutes(_settings.TokenCacheMinutes));

//            return token;
//        }
//        #endregion
//    }
//}