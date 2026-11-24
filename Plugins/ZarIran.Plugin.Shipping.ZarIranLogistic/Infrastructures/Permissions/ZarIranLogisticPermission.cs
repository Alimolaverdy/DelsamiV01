using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Infrastructures.Permissions
{
    public class ZarIranLogisticPermission : IPermissionProvider
    {
        public static readonly PermissionRecord ManageZarIranLogistic = new PermissionRecord
        {
            Name = "Admin area. ManageZarIranLogistic",
            SystemName = "ManageZarIranLogistic",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageZarIranLogistic,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageZarIranLogistic }),

        };
        }
    }
}
