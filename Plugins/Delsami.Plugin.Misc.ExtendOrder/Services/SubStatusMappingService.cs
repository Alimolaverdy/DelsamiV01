using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Services
{
    public class SubStatusMappingService : ISubStatusMappingService
    {
        #region Fields
        private readonly IRepository<OrderSubStatusMapping> _repo;
        #endregion

        #region Ctor
        public SubStatusMappingService(IRepository<OrderSubStatusMapping> repo)
        {
            _repo = repo;
        }
        #endregion

        #region Methods
        public async Task<OrderSubStatusMapping> GetLastOrderSubStatusAsync(int id)
        {
            return await _repo.Table.Where(x => x.OrderId == id).OrderByDescending(o => o.CreatedDateTime).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(OrderSubStatusMapping orderSubStatus)
        {
            await _repo.InsertAsync(orderSubStatus);
        }
        #endregion
    }
}
