using Delsami.Plugin.Shipping.Tipax.Models.TipaxApi;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Tipax.Factories
{
    public interface ITipaxModelsFactory
    {
        Task<PricingRequestModel> PreparePricingRequestModelAsync(int destinationCityId,
                                                                  decimal weight,
                                                                  decimal packageValue,
                                                                  decimal length,
                                                                  decimal width,
                                                                  decimal height);

        TokenRequestModel PrepareTokenRequestModelAsync();
    }
}
