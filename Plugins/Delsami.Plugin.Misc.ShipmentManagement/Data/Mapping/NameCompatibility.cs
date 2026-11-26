using Delsami.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Data.Mapping;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Misc.ShipmentManagement.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ShipmentDelivery), $"D_SM_{nameof(ShipmentDelivery)}"},
            { typeof(ShipmentTrackingNumberImport), $"D_SM_{nameof(ShipmentTrackingNumberImport)}"},
            { typeof(ShipmentTrackingNumberImporterReport), $"D_SM_{nameof(ShipmentTrackingNumberImporterReport)}"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
