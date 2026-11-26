using FluentMigrator;
using Delsami.Plugin.Payments.Digipay.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Payments.Digipay.Data
{
	[NopMigration("2026/11/26 09:38:00", "Delsami.Plugin.Payments.DigiPay add refund record")]
	public class SchemaMigration : AutoReversingMigration
	{
		public override void Up()
		{
			Create.TableFor<DigiPayRefundRecord>();

		}
	}
}
