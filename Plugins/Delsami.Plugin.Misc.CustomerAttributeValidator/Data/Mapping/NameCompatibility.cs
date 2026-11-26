using Delsami.Plugin.Misc.CustomerAttributeValidator.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Data.Mapping
{

    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(CustomerAttributeValidation), $"D_CAV_{nameof(CustomerAttributeValidation)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}