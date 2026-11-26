using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Core.Domain.Catalog;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public interface IReservedStockQuantityHistoryService
    {
        Task InsertAsync(Product product,
                         ReservedStockQuantity reservedStockQuantity,
                         string orderId,
                         int countAdjustment,
                         string message);
    }
}
