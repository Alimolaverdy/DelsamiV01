using ZarIran.Plugin.Misc.ShipmentManagement.Models;
using Nop.Core.Domain.Orders;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Factories
{
    public interface IZarIranOrderModelFactory
    {
        Task<ZarIranOrderDetailsModel> PrepareOrderDetailsModelAsync(Order order);
        Task<ZarIranCustomerOrderListModel> PrepareCustomerOrderListModelAsync();

    }
}
