using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Services
{
    public interface IOrderInvoiceService
    {
        Task InsertAsync(OrderInvoice orderInvoice);

        Task<OrderInvoice> GetOrderInvoiceByOrderIdAsync(int orderId);

    }
}
