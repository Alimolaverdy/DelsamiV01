using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Models.PriceRanges;
using ZarIran.Plugin.Shipping.IRPost.Domain;
using ZarIran.Plugin.Shipping.IRPost.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.IRPost.Areas.Admin.Factories
{
    public class PriceRangeModelFactory : IPriceRangeModelFactory
    {
        #region Fields

        private readonly IPriceRangeService _priceRangeService;

        #endregion

        #region Ctor

        public PriceRangeModelFactory(IPriceRangeService priceRangeService)
        {
            _priceRangeService = priceRangeService;
        }

        #endregion

        #region Methods

        public virtual PriceRangeSearchModel PreparePriceRangeSearchModel(PriceRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual PriceRangeModel PreparePriceRangeModel(PriceRangeModel model, PriceRange priceRange)
        {
            if (model == null)
            {
                model = new PriceRangeModel();

                if (priceRange != null)
                    model = priceRange.ToModel<PriceRangeModel>();
            }

            return model;
        }

        public virtual async Task<PriceRangeListModel> PreparePriceRangeListModelAsync(PriceRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var priceRanges = await _priceRangeService.GetPriceRangesAsync(pageIndex: searchModel.Page - 1,
                                                                            pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new PriceRangeListModel().PrepareToGrid(searchModel, priceRanges, () =>
            {
                return priceRanges.Select(customerCar =>
                {
                    //fill in model values from the entity
                    var priceRangeModel = customerCar.ToModel<PriceRangeModel>();
                    
                    return priceRangeModel;
                });
            });

            return model;
        }

        #endregion
    }
}
