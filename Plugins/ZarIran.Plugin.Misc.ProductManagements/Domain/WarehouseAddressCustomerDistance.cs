using Nop.Core;

namespace ZarIran.Plugin.Misc.ProductManagements.Domain
{
    public class WarehouseAddressCustomerDistance : BaseEntity
    {
        public int WarehouseAddressId { get; set; }

        public int CustomerAddressId { get; set; }

        public int Distance { get; set; }
    }
}
