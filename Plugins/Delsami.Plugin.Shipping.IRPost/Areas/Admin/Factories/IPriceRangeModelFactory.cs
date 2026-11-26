using Delsami.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using Delsami.Plugin.Shipping.IRPost.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost.Areas.Admin.Factories
{
    public interface IPriceRangeModelFactory
    {
        PriceRangeSearchModel PreparePriceRangeSearchModel(PriceRangeSearchModel searchModel);

        PriceRangeModel PreparePriceRangeModel(PriceRangeModel model, PriceRange priceRange);

        Task<PriceRangeListModel> PreparePriceRangeListModelAsync(PriceRangeSearchModel searchModel);
    }
}
