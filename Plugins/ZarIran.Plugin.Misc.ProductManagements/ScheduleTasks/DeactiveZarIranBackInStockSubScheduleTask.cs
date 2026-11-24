using ZarIran.Plugin.Misc.ProductManagements.Services;
using Nop.Services.ScheduleTasks;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.ScheduleTasks
{
    public  class DeactiveZarIranBackInStockSubScheduleTask : IScheduleTask
    {
        private readonly IZarIranBackInStockSubscriptionService _ZarIranIBackInStockSubscriptionService;

        public DeactiveZarIranBackInStockSubScheduleTask(IZarIranBackInStockSubscriptionService ZarIranIBackInStockSubscriptionService)
        {
            _ZarIranIBackInStockSubscriptionService = ZarIranIBackInStockSubscriptionService;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                var unsentBackInStockSubscriptions = await _ZarIranIBackInStockSubscriptionService.GetAllValidForDeActiveAsync();
                foreach (var item in unsentBackInStockSubscriptions)
                {
                    await _ZarIranIBackInStockSubscriptionService.DeActiveAsync(item);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
