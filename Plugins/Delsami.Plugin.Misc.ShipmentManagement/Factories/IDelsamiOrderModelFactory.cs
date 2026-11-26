using Delsami.Plugin.Misc.ShipmentManagement.Models;
using Nop.Core.Domain.Orders;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ShipmentManagement.Factories
{
    public interface IDelsamiOrderModelFactory
    {
        Task<DelsamiOrderDetailsModel> PrepareOrderDetailsModelAsync(Order order);
        Task<DelsamiCustomerOrderListModel> PrepareCustomerOrderListModelAsync();

    }
}
