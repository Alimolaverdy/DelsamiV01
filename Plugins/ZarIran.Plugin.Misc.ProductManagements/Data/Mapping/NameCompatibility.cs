using System;
using System.Collections.Generic;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(WarehouseAddressCustomerDistance), $"ZI_PMS_{nameof(WarehouseAddressCustomerDistance)}"},
            { typeof(WarehouseVendor), $"ZI_PMS_{nameof(WarehouseVendor)}"},
            { typeof(GeneratedVirtualProduct), $"ZI_PMS_{nameof(GeneratedVirtualProduct)}"},
            { typeof(ReservedStockQuantity), $"ZI_PMS_{nameof(ReservedStockQuantity)}"},
            { typeof(ReservedStockQuantityHistory), $"ZI_PMS_{nameof(ReservedStockQuantityHistory)}"},
            { typeof(ZarIranBackInStockSubscription), $"ZI_PMS_{nameof(ZarIranBackInStockSubscription)}"},
            { typeof(OtherBrandProduct), $"ZI_PMS_{nameof(OtherBrandProduct)}"},
            { typeof(BundleProduct), $"ZI_PMS_{nameof(BundleProduct)}"},
            { typeof(ProductPricing), $"ZI_PMS_{nameof(ProductPricing)}"},

        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
