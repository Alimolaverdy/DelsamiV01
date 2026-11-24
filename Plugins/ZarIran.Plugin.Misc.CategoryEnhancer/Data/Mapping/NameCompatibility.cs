using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(CategoryIcon), $"ZI_CE_{nameof(CategoryIcon)}_Mapping"},
            { typeof(CanonicalCategory), $"ZI_CE_{nameof(CanonicalCategory)}"},
            { typeof(CategorySpecificationAttribute), $"ZI_CE_{nameof(CategorySpecificationAttribute)}_Mapping"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
