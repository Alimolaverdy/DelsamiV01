using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using PaymentDomain = ZarIran.Plugin.Payment.Core.Domain;

namespace ZarIran.Plugin.Payment.Core.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(PaymentDomain.Payment), $"ZI_P_{nameof(PaymentDomain.Payment)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
