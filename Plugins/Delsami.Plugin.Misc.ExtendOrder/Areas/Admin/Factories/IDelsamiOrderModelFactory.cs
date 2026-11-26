using Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories
{
    public interface IDelsamiOrderModelFactory
    {
        Task<OrderSearchModel> PrepareOrderSearchModelAsync(DelsamiOrderSearchModel searchModel);
        Task<DelsamiOrderListModel> PrepareOrderListModelAsync(DelsamiOrderSearchModel searchModel);
        Task<DelsamiOrderModel> PrepareOrderModelAsync(DelsamiOrderModel model, Order order, bool excludeProperties = false);
        Task<DelsamiOrderAggreratorModel> PrepareOrderAggregatorModelAsync(OrderSearchModel searchModel);
    }
}
