using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Infrastructure.Permissions
{
    public class ExtendedOrderPermission : IPermissionProvider
    {
        public static readonly PermissionRecord ManageExtendedOrder = new PermissionRecord
        {
            Name = "Admin area. ManageExtendedOrder",
            SystemName = "ManageExtendedOrder",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageExtendedOrder,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageExtendedOrder }),

        };
        }
    }
}
