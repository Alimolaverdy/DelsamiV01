using Delsami.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(CategoryIcon), $"D_CE_{nameof(CategoryIcon)}_Mapping"},
            { typeof(CanonicalCategory), $"D_CE_{nameof(CanonicalCategory)}"},
            { typeof(CategorySpecificationAttribute), $"D_CE_{nameof(CategorySpecificationAttribute)}_Mapping"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
