using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(OrderInvoice), $"ZI_EO_{nameof(OrderInvoice)}"},
            { typeof(OrderSubStatus), $"ZI_EO_{nameof(OrderSubStatus)}"},
            { typeof(OrderSubStatusMapping), $"ZI_EO_{nameof(OrderSubStatusMapping)}"},
            { typeof(OrderShippingMethod), $"ZI_EO_{nameof(OrderShippingMethod)}"},
            { typeof(OrderWarehouseLocation), $"ZI_EO_{nameof(OrderWarehouseLocation)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
