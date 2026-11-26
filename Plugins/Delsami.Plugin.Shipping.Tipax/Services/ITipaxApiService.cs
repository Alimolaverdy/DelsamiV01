using Delsami.Plugin.Shipping.Tipax.Models.TipaxApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Tipax.Services
{
    public interface ITipaxApiService
    {
        Task<List<PricingResultModel>> GetPricingAsync(PricingRequestModel model, string accessToken);

        Task<TokenResponseModel> GetTokenAsync(TokenRequestModel tokenRequest);
    }
}
