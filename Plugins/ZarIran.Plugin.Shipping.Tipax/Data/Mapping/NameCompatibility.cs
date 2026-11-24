using ZarIran.Plugin.Shipping.Tipax.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.Tipax.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ZarIranTipaxCityMapping), $"ZI_T_{nameof(ZarIranTipaxCityMapping)}"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
