using Nop.Core;

namespace Delsami.Plugin.Misc.ProductManagement.Domain
{
    public class WarehouseAddressCustomerDistance : BaseEntity
    {
        public int WarehouseAddressId { get; set; }

        public int CustomerAddressId { get; set; }

        public int Distance { get; set; }
    }
}
