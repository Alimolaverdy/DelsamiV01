using ZarIran.Plugin.Shipping.ZarIranLogistic.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(QueuedCanceledShipmentForSendToLogistic), $"ZI_ZL_{nameof(QueuedCanceledShipmentForSendToLogistic)}"}
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
