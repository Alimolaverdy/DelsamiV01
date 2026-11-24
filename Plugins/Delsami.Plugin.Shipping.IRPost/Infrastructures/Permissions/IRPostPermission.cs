using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Shipping.IRPost.Infrastructures.Permissions
{
    public class IRPostPermission : IPermissionProvider
    {
        public static readonly PermissionRecord ManageIRPost = new PermissionRecord
        {
            Name = "Admin area. ManageIRPost",
            SystemName = "ManageIRPost",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageIRPost,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageIRPost }),

        };
        }
    }
}
