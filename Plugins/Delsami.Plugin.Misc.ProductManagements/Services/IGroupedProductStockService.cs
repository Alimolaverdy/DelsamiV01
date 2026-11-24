using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface IGroupedProductStockService
    {
          Task RecalculateParentStockAsync(int parentGroupedProductId);
          Task UpdateParentStockIncrementalAsync(int childProductId, int quantityDelta);
    }
}
