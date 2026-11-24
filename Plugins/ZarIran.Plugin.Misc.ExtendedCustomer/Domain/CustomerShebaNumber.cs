using Nop.Core;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Domain
{
    public class CustomerShebaNumber : BaseEntity
    {
        public int CustomerId { get; set; }

        public string Number { get; set; }
    }
}
