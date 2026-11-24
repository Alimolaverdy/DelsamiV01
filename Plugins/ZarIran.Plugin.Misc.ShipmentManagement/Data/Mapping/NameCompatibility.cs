using ZarIran.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ShipmentDelivery), $"ZI_SM_{nameof(ShipmentDelivery)}"},
            { typeof(ShipmentTrackingNumberImport), $"ZI_SM_{nameof(ShipmentTrackingNumberImport)}"},
            { typeof(ShipmentTrackingNumberImporterReport), $"ZI_SM_{nameof(ShipmentTrackingNumberImporterReport)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
