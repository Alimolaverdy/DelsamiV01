using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public interface IOrderWarehouseLocationService
    {
        Task InsertAsync(OrderWarehouseLocation orderWarehouseLocation);
    }
}
