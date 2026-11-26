using Nop.Core;

namespace Delsami.Plugin.Misc.ExtendedOrder.Domian
{
    public class OrderInvoice : BaseEntity
    {
        public int OrderId { get; set; }
        public int InvoiceId { get; set; }

    }
}
