using System;
using System.Collections.Generic;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Data.Mapping;

namespace Delsami.Plugin.Misc.ProductManagement.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(WarehouseAddressCustomerDistance), $"D_PM_{nameof(WarehouseAddressCustomerDistance)}"},
            { typeof(WarehouseVendor), $"D_PM_{nameof(WarehouseVendor)}"},
            { typeof(GeneratedVirtualProduct), $"D_PM_{nameof(GeneratedVirtualProduct)}"},
            { typeof(ReservedStockQuantity), $"D_PM_{nameof(ReservedStockQuantity)}"},
            { typeof(ReservedStockQuantityHistory), $"D_PM_{nameof(ReservedStockQuantityHistory)}"},
            { typeof(BundleProduct), $"D_PM_{nameof(BundleProduct)}"},

        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
