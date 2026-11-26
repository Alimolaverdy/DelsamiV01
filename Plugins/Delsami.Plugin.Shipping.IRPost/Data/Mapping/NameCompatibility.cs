using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.IRPost.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {

        { typeof(PriceRange), $"D_IP_{nameof(PriceRange)}"},
        { typeof(DelsamiPostCityMapping), $"D_IP_{nameof(DelsamiPostCityMapping)}"}

        };
        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}