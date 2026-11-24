using ZarIran.Plugin.Misc.ProductManagements.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface IZarIranBackInStockSubscriptionService
    {
        Task InsertAsync(ZarIranBackInStockSubscription backInStockSubscription);
        Task<ZarIranBackInStockSubscription> GetByCustomerAndProductIdAsync(int customerId, int productId);
        Task<ZarIranBackInStockSubscription> GetByMobileNumberAndProductIdAsync(string mobileNumber, int productId);
        Task<(int totalCount, int lastHourCount)> GetSubscriptionCountsAsync(string username, int productId);
        Task<List<ZarIranBackInStockSubscription>> GetAllValidForNotifiedByProductIdsAsync(List<int> productIds);
        Task<List<ZarIranBackInStockSubscription>> GetAllValidForDeActiveAsync();
        Task DeActiveAsync(ZarIranBackInStockSubscription backInStockSubscription);
        Task NotifiedAsync(ZarIranBackInStockSubscription backInStockSubscription);
    }
}
