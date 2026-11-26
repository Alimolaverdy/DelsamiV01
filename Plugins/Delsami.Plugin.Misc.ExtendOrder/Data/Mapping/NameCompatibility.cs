using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ExtendedOrder.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(OrderInvoice), $"D_EO_{nameof(OrderInvoice)}"},
            { typeof(OrderSubStatus), $"D_EO_{nameof(OrderSubStatus)}"},
            { typeof(OrderSubStatusMapping), $"D_EO_{nameof(OrderSubStatusMapping)}"},
            { typeof(OrderShippingMethod), $"D_EO_{nameof(OrderShippingMethod)}"},
            { typeof(OrderWarehouseLocation), $"D_EO_{nameof(OrderWarehouseLocation)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
