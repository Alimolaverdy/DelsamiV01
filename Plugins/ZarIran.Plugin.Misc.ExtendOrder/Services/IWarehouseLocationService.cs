using ZarIran.Plugin.Misc.ExtendedOrder.Model.WarehouseLocations;
using System.Threading;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Services
{
    public interface IWarehouseLocationService
    {
        Task<WarehouseLocationResponseModel> GetOrderWarehouseFromFulfillmentAsync(WarehouseLocationRequestModel orderRequest);
    }
}
