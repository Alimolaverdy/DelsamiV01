using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.ProductManagement.Domain
{
    public class WarehouseVendor : BaseEntity
    {
        public int VendorId { get; set; }

        public int WarehouseId { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
