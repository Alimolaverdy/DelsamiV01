using Delsami.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Core;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.IRPost.Services
{
    public class PriceRangeService : IPriceRangeService
    {
        #region Fields

        private readonly IRepository<PriceRange> _priceRangeRepo;

        #endregion

        #region Ctor

        public PriceRangeService(IRepository<PriceRange> priceRangeRepo)
        {
            _priceRangeRepo = priceRangeRepo;
        }

        #endregion

        #region Methods

        public virtual async Task<(bool IsValid, WeightRangeValidationError? Error)> IsValidAsync(PriceRange priceRange)
        {
            var minRangeInvalid = false;
            var maxRangeInvalid = false;

            var basedOnMinWeight = await _priceRangeRepo.Table.FirstOrDefaultAsync(w => !w.Deleted && w.MinWieght < priceRange.MinWieght && (w.MaxWeight > priceRange.MinWieght || w.ToUpperWeight) && priceRange.Id != w.Id);

            if (basedOnMinWeight is not null)
                minRangeInvalid = true;

            var basedOnMaxWeight = await _priceRangeRepo.Table.FirstOrDefaultAsync(w => !w.Deleted && w.MinWieght < priceRange.MaxWeight && (w.MaxWeight > priceRange.MaxWeight || w.ToUpperWeight) && priceRange.Id != w.Id);

            if (basedOnMaxWeight is not null)
                maxRangeInvalid = true;

            var basedOnTotalWeight = await _priceRangeRepo.Table.FirstOrDefaultAsync(w => !w.Deleted && (w.MaxWeight < priceRange.MaxWeight || priceRange.ToUpperWeight) && (w.MinWieght > priceRange.MinWieght) && priceRange.Id != w.Id);

            if ((minRangeInvalid && maxRangeInvalid) || basedOnTotalWeight != null)
                return (false, WeightRangeValidationError.MinAndMaxInvalid);

            else if (minRangeInvalid)
                return (false, WeightRangeValidationError.MinPriceInvalid);

            else if (maxRangeInvalid)
                return (false, WeightRangeValidationError.MaxPriceInvalid);
            else
                return (true, null);
        }

        public virtual async Task InsertAsync(PriceRange priceRange)
        {
            await _priceRangeRepo.InsertAsync(priceRange);
        }

        //public virtual async Task UpdateAsync(PriceRange priceRange)
        //{
        //    await _priceRangeRepo.UpdateAsync(priceRange);
        //}

        public virtual async Task DeleteAsync(PriceRange priceRange)
        {
            priceRange.Deleted = true;

            await _priceRangeRepo.UpdateAsync(priceRange);
        }

        public virtual async Task<PriceRange> GetPriceRangeAsync(int id)
        {
            var priceRange = await _priceRangeRepo.Table.FirstOrDefaultAsync(f => !f.Deleted && f.Id == id);

            return priceRange;
        }

        public virtual async Task<IPagedList<PriceRange>> GetPriceRangesAsync(int pageIndex = 0,
                                                                              int pageSize = int.MaxValue)
        {
            var priceRanges = await _priceRangeRepo.GetAllPagedAsync(query =>
            {

                return query.Where(w => !w.Deleted).OrderBy(c => c.MinWieght);

            }, pageIndex, pageSize);

            return priceRanges;
        }

        public virtual async Task<decimal?> GetPriceRangeAsync(decimal weight, bool isNeighbouring = false)
        {
            var priceRange = (await _priceRangeRepo.Table.FirstOrDefaultAsync(f => !f.Deleted && f.MinWieght <= weight && (f.MaxWeight > weight || f.ToUpperWeight)));

            if (priceRange is not null)
            {
                if (isNeighbouring)
                    return priceRange.NeighbouringPrice;
                else
                    return priceRange.NonNeighbouringPrice;
            }

            return null;
        }

        #endregion
    }
}
