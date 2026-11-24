using ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using ZarIran.Plugin.Misc.ExtendedOrder.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories
{
    public class SubStatusFactory : ISubStatusFactory
    {
        private readonly ISubStatusService _subStatusService;

        public SubStatusFactory(ISubStatusService subStatusService)
        {
            _subStatusService = subStatusService;
        }

        public virtual OrderSubStatusSearchModel PrepareOrderSubStatusSearchModel(OrderSubStatusSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }


        public virtual OrderSubStatusModel PrepareOrderSubStatusModel(OrderSubStatusModel model, OrderSubStatus orderSubStatus)
        {
            if (model == null)
            {
                model = new OrderSubStatusModel();

                if (orderSubStatus != null)
                    model = orderSubStatus.ToModel<OrderSubStatusModel>();
            }

            return model;
        }


        public virtual async Task<OrderSubStatusListModel> PrepareOrderSubStatusListModelAsync(OrderSubStatusSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var subStatuses = await _subStatusService.GetSubStatusesAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new OrderSubStatusListModel().PrepareToGrid(searchModel, subStatuses, () =>
            {
                return subStatuses.Select( smsReceiver =>
                {
                    //fill in model values from the entity
                    var orderSubStatusModel = smsReceiver.ToModel<OrderSubStatusModel>();
                    return orderSubStatusModel;
                });
            });

            return model;
        }
    }
}
