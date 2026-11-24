using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Core.Domain.Customers;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public class ZarIranBackInStockSubscriptionService : IZarIranBackInStockSubscriptionService
    {

        private readonly IRepository<ZarIranBackInStockSubscription> _repository;
        private readonly ProductManagementSettings _productManagementSettings;


        public ZarIranBackInStockSubscriptionService(IRepository<ZarIranBackInStockSubscription> repository, ProductManagementSettings productManagementSettings)
        {
            _repository = repository;
            _productManagementSettings = productManagementSettings;
        }

        public async Task DeActiveAsync(ZarIranBackInStockSubscription backInStockSubscription)
        {
            backInStockSubscription.IsActive = false;
            backInStockSubscription.UpdatedOnUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(backInStockSubscription);
        }

        public async Task<List<ZarIranBackInStockSubscription>> GetAllValidForNotifiedByProductIdsAsync(List<int> productIds)
        {
            var query = _repository.Table;

            return await query
           .Where(x =>
                 productIds.Contains(x.ProductId) &&
                 !x.IsNotified &&
                 x.IsActive &&
                  x.CreatedOnUtc.AddDays(_productManagementSettings.BackInStockSubscriptionNotifiedPeriodInDays) > DateTime.UtcNow)
           .ToListAsync();
        }

        public async Task<ZarIranBackInStockSubscription> GetByCustomerAndProductIdAsync(int customerId, int productId)
        {
            var query = _repository.Table;
            return await query.OrderByDescending(x => x.UpdatedOnUtc)
                .FirstOrDefaultAsync(i =>
                    i.CustomerId == customerId &&
                    i.ProductId == productId);
        }

        public async Task<ZarIranBackInStockSubscription> GetByMobileNumberAndProductIdAsync(string mobileNumber, int productId)
        {
            var query = _repository.Table;
            return await query.OrderByDescending(x => x.UpdatedOnUtc)
                .FirstOrDefaultAsync(i =>
                    i.MobileNumber == mobileNumber &&
                    i.ProductId == productId);
        }

        public async Task InsertAsync(ZarIranBackInStockSubscription backInStockSubscription)
        {
            if (backInStockSubscription == null)
                throw new ArgumentNullException(nameof(backInStockSubscription));

            await _repository.InsertAsync(backInStockSubscription);
        }

        public async Task NotifiedAsync(ZarIranBackInStockSubscription backInStockSubscription)
        {
            backInStockSubscription.IsNotified = true;
            backInStockSubscription.IsActive = false;
            backInStockSubscription.UpdatedOnUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(backInStockSubscription);
        }

        public async Task<List<ZarIranBackInStockSubscription>> GetAllValidForDeActiveAsync()
        {
            var query = _repository.Table;

            return await query
           .Where(x =>
                 !x.IsNotified &&
                  x.IsActive &&
                  x.CreatedOnUtc.AddDays(_productManagementSettings.BackInStockSubscriptionActivePeriodInDays) < DateTime.UtcNow)
           .ToListAsync();
        }

        public async Task<(int totalCount, int lastHourCount)> GetSubscriptionCountsAsync(string username, int productId)
        {
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);

            var result = await _repository.Table
                .Where(x => x.MobileNumber == username
                         && x.IsActive
                         && !x.IsNotified)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    TotalCount = g.Count(),
                    LastHourCount = _repository.Table
                        .Count(x => x.MobileNumber == username
                                 && x.ProductId == productId
                                 && x.CreatedOnUtc >= oneHourAgo)
                })
                .FirstOrDefaultAsync();

            return (result?.TotalCount ?? 0, result?.LastHourCount ?? 0);
        }
    }
}
