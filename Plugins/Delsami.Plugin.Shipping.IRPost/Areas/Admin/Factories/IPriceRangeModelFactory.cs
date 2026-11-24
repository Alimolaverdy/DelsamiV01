using ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using ZarIran.Plugin.Shipping.IRPost.Domain;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Factories
{
    public interface IPriceRangeModelFactory
    {
        PriceRangeSearchModel PreparePriceRangeSearchModel(PriceRangeSearchModel searchModel);

        PriceRangeModel PreparePriceRangeModel(PriceRangeModel model, PriceRange priceRange);

        Task<PriceRangeListModel> PreparePriceRangeListModelAsync(PriceRangeSearchModel searchModel);
    }
}
