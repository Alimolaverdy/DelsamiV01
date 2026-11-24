using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.AdvRedirect.Infrastructure.Permissions
{
    public class AdvRedirectPermission : IPermissionProvider
    {
        public static readonly PermissionRecord ManageRedirectionRule = new PermissionRecord
        {
            Name = "Admin area. Manage Redirection Rule",
            SystemName = "ManageRedirectionRule",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageRedirectionRule,
            };
        }


        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageRedirectionRule }),

        };
        }
    }
}
