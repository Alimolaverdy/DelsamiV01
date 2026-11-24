using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.TorobPay.Infrastructures.Permissions
{
    public class TorobPayPermissions : IPermissionProvider
    {
        public static readonly PermissionRecord ManageTorobPay = new PermissionRecord
        {
            Name = "Admin area. ManageTorobPay",
            SystemName = "ManageTorobPay",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageTorobPay,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageTorobPay }),

        };
        }
    }
}
