using ZarIran.Plugin.Shipping.ManageShippingMethod.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ShippingMethodCityRestriction), $"ZI_MS_{nameof(ShippingMethodCityRestriction)}"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
