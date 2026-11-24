using ZarIran.Plugin.Shipping.Tipax.Models.TipaxApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.Tipax.Services
{
    public interface ITipaxApiService
    {
        Task<List<PricingResultModel>> GetPricingAsync(PricingRequestModel model, string accessToken);

        Task<TokenResponseModel> GetTokenAsync(TokenRequestModel tokenRequest);
    }
}
