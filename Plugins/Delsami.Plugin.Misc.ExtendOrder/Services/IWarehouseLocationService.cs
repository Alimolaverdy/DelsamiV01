using Delsami.Plugin.Misc.ExtendedOrder.Model.WarehouseLocations;
using System.Threading;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public interface IWarehouseLocationService
    {
        Task<WarehouseLocationResponseModel> GetOrderWarehouseFromFulfillmentAsync(WarehouseLocationRequestModel orderRequest);
    }
}
