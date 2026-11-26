using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public interface IReservedStockQuantityService
    {
        Task<int> GetReservedStockQuantityCountAsync(int productId);

        Task<ReservedStockQuantity> GetReservedStockQuantityAsync(int productId);

        Task InsertAsync(ReservedStockQuantity reservedStockQuantity);

        Task UpdateAsync(ReservedStockQuantity reservedStockQuantity);

        Task AdjustReservedStockQuantityAsync(Product product, string orderId, int quantity, string message);

        Task ReverseReservedStockQuantityAsync(Order order, string message);

    }
}
