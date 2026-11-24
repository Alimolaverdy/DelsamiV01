using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Services
{
    public interface IOrderWarehouseLocationService
    {
        Task InsertAsync(OrderWarehouseLocation orderWarehouseLocation);
    }
}
