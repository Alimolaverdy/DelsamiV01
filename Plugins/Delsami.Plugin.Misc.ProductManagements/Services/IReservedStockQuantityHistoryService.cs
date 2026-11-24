using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core.Domain.Catalog;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
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
