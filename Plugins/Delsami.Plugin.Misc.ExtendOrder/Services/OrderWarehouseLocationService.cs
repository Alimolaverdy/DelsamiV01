using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data;
using System;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Services
{
    public class OrderWarehouseLocationService : IOrderWarehouseLocationService
    {
        #region Ctor

        private readonly IRepository<OrderWarehouseLocation> _orderWarehouseLocationRepo;

        #endregion

        #region Ctor

        public OrderWarehouseLocationService(IRepository<OrderWarehouseLocation> orderWarehouseLocationRepo)
        {
            _orderWarehouseLocationRepo = orderWarehouseLocationRepo;
        }

        #endregion

        #region Methods

        public async Task InsertAsync(OrderWarehouseLocation orderWarehouseLocation)
        {
            if (orderWarehouseLocation is null)
                throw new ArgumentNullException(nameof(orderWarehouseLocation));

            await _orderWarehouseLocationRepo.InsertAsync(orderWarehouseLocation);
        }

        #endregion
    }
}
