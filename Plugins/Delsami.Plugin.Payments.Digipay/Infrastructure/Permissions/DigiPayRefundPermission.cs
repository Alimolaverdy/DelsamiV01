using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Delsami.Plugin.Payments.Digipay.Infrastructure.Permissions
{
	public class DigiPayRefundPermission : IPermissionProvider
	{
		public static readonly PermissionRecord DigiPayRefund = new PermissionRecord
		{
			Name = "Admin area. DigiPayRefund",
			SystemName = "DigiPayRefund",
			Category = "Plugin"
		};

		public IEnumerable<PermissionRecord> GetPermissions()
		{
			return new[]
			{
				DigiPayRefund,
			};
		}
		public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
		{
			return new HashSet<(string systemRoleName, PermissionRecord[] permissions)>
		{
			("Administrators", new[] { DigiPayRefund }),

		};
		}
	}
}
