using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(CustomerShebaNumber), $"ZI_EC_{nameof(CustomerShebaNumber)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
