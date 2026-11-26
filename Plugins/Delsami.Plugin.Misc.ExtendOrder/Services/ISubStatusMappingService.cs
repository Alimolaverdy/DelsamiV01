using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public interface ISubStatusMappingService
    {
        Task InsertAsync(OrderSubStatusMapping orderSubStatus);
        Task<OrderSubStatusMapping> GetLastOrderSubStatusAsync(int id);
    }
}
