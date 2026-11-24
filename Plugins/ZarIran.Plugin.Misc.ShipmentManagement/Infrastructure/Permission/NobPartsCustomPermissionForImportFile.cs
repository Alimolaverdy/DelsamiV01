using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Infrastructure.Permission;

public class ZarIranCustomPermissionForImportFile : IPermissionProvider
{

    public static readonly PermissionRecord ImportShippmentTrackNumberFromFile = new PermissionRecord
    {
        Name = "Admin area. Allow Import Shipment Tracking Number From File",
        SystemName = "ManageImportShipmentTrackingNumberFromFile",
        Category = "Plugins"
    };


    public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
    {
        return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ImportShippmentTrackNumberFromFile }),

        };
    }

    public IEnumerable<PermissionRecord> GetPermissions()
    {
        return new[] { ImportShippmentTrackNumberFromFile };

    }
}
