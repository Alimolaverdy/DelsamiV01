using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Services
{
    public class OrderInvoiceService : IOrderInvoiceService
    {
        #region Fields

        private readonly IRepository<OrderInvoice> _orderInvoiceRepository;

        #endregion

        #region Ctor

        public OrderInvoiceService(IRepository<OrderInvoice> orderInvoiceRepository)
        {
            _orderInvoiceRepository = orderInvoiceRepository;
        }

        #endregion

        #region Methods

        public async Task InsertAsync(OrderInvoice orderInvoice)
        {
            if (orderInvoice == null)
                throw new ArgumentNullException(nameof(orderInvoice));

            await _orderInvoiceRepository.InsertAsync(orderInvoice);
        }

        public async Task<OrderInvoice> GetOrderInvoiceByOrderIdAsync(int orderId)
        {
            var orderInvoice = await _orderInvoiceRepository.Table.FirstOrDefaultAsync(w => w.OrderId == orderId);

            return orderInvoice;
        }

        #endregion
    }
}
