using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public interface IGroupedProductStockService
    {
          Task RecalculateParentStockAsync(int parentGroupedProductId);
          Task UpdateParentStockIncrementalAsync(int childProductId, int quantityDelta);
    }
}
