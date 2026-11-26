using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Core;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public class SubStatusService : ISubStatusService
    {
        private readonly IRepository<OrderSubStatus> _repo;
        private readonly ExtendedOrderSettings _extendedOrderSettings;


        public SubStatusService(IRepository<OrderSubStatus> repo, ExtendedOrderSettings extendedOrderSettings)
        {
            _repo = repo;
            _extendedOrderSettings = extendedOrderSettings;
        }

        public async Task InsertAsync(OrderSubStatus orderSubStatus)
        {
            await _repo.InsertAsync(orderSubStatus);
        }

        public async Task<OrderSubStatus> GetOrderSubStatusAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(OrderSubStatus smsReceiverInfo)
        {
            await _repo.UpdateAsync(smsReceiverInfo);

        }

        public async Task DeleteAsync(OrderSubStatus orderSubStatus)
        {
            await _repo.DeleteAsync(orderSubStatus);
        }

        public virtual async Task<IPagedList<OrderSubStatus>> GetSubStatusesAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool excludeSystem = false)
        {
            var orderSubStatus = await _repo.GetAllPagedAsync(query =>
            {
                if (excludeSystem)
                    query = query.Where(c => c.Id != _extendedOrderSettings.systemSubStatusId);

                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return orderSubStatus;
        }

    }
}
