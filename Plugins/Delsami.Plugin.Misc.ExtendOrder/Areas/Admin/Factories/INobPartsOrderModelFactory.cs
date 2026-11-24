using ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Factories
{
    public interface IZarIranOrderModelFactory
    {
        Task<OrderSearchModel> PrepareOrderSearchModelAsync(ZarIranOrderSearchModel searchModel);
        Task<ZarIranOrderListModel> PrepareOrderListModelAsync(ZarIranOrderSearchModel searchModel);
        Task<ZarIranOrderModel> PrepareOrderModelAsync(ZarIranOrderModel model, Order order, bool excludeProperties = false);
        Task<ZarIranOrderAggreratorModel> PrepareOrderAggregatorModelAsync(OrderSearchModel searchModel);
    }
}
