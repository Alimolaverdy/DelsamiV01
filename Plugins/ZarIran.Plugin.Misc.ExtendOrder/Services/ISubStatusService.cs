using ZarIran.Plugin.Misc.ExtendedOrder.Areas.Admin.Models;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Core;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Services
{
    public interface ISubStatusService
    {
        Task<IPagedList<OrderSubStatus>> GetSubStatusesAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool excludeSystem = false);
        Task InsertAsync(OrderSubStatus orderSubStatus);
        Task<OrderSubStatus> GetOrderSubStatusAsync(int id);
        Task UpdateAsync(OrderSubStatus orderSubStatus);
        Task DeleteAsync(OrderSubStatus orderSubStatus);
    }
}
