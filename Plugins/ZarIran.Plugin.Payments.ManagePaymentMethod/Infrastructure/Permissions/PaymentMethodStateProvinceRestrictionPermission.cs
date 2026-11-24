using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Infrastructure.Permissions
{
    public class PaymentMethodStateProvinceRestrictionPermission : IPermissionProvider
    {

        public static readonly PermissionRecord ManagePaymentMethod = new PermissionRecord
        {
            Name = "Admin area. ManagePaymentMethod",
            SystemName = "ManagePaymentMethod",
            Category = "Plugin"
        };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                ManagePaymentMethod,
            };
        }
        public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
        {
            ("Administrators", new[] { ManagePaymentMethod }),

        };
        }
    }
}
