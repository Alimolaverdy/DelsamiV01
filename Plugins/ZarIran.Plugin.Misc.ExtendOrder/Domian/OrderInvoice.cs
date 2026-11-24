using Nop.Core;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Domian
{
    public class OrderInvoice : BaseEntity
    {
        public int OrderId { get; set; }
        public int InvoiceId { get; set; }

    }
}
