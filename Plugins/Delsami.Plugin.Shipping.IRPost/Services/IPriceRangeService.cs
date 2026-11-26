using Delsami.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Core;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost.Services
{
    public interface IPriceRangeService
    {
        Task InsertAsync(PriceRange priceRange);

        //Task UpdateAsync(PriceRange priceRange);

        Task DeleteAsync(PriceRange priceRange);

        Task<PriceRange> GetPriceRangeAsync(int id);

        Task<IPagedList<PriceRange>> GetPriceRangesAsync(int pageIndex = 0,
                                                         int pageSize = int.MaxValue);

        Task<(bool IsValid, WeightRangeValidationError? Error)> IsValidAsync(PriceRange priceRange);

        Task<decimal?> GetPriceRangeAsync(decimal weight, bool isNeighbouring = false);
    }
}
