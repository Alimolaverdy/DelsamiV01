using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.ManageShippingMethod.Infrastructures.Permissions
{
    public class ShippingMethodStateProvinceRestrictionPermission : IPermissionProvider
    {

        public static readonly PermissionRecord ManageShippingMethod = new PermissionRecord
        {
            Name = "Admin area. ManageShippingMethod",
            SystemName = "ManageShippingMethod",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManageShippingMethod,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManageShippingMethod }),

        };
        }
    }
}
