using Delsami.Plugin.Misc.ShipmentManagement.Areas.Admin.Models.Shipments;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement.Areas.Admin.Factories
{
    public interface IDelsamiShipmentModelFactory
    {
        Task<DelsamiOrderShipmentListModel> PrepareOrderShipmentListModelAsync(OrderShipmentSearchModel searchModel, Order order);
    }
}
