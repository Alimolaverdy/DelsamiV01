using ZarIran.Plugin.Shipping.Core.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.Core.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(Carton), $"ZI_EO_{nameof(Carton)}"},
            { typeof(LogisticsSchedule), $"ZI_EO_{nameof(LogisticsSchedule)}"},
            { typeof(LogisticsScheduleException), $"ZI_SC_{nameof(LogisticsScheduleException)}"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
