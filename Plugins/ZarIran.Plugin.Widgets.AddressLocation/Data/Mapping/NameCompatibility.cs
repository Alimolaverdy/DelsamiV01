using ZarIran.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Widgets.AddressLocation.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(StateProvinceLocation), $"ZI_AL_{nameof(StateProvinceLocation)}"},
            { typeof(AddressCity), $"ZI_AL_{nameof(AddressCity)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
