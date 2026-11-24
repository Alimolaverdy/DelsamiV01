using ZarIran.Plugin.Shipping.IRPost.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.IRPost.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {

        { typeof(PriceRange), $"ZI_IP_{nameof(PriceRange)}"},
        { typeof(ZarIranPostCityMapping), $"ZI_IP_{nameof(ZarIranPostCityMapping)}"}

        };
        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}