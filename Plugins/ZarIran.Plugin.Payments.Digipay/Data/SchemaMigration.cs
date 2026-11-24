using FluentMigrator;
using ZarIran.Plugin.Payments.Digipay.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Payments.Digipay.Data
{
	[NopMigration("2026/06/16 09:38:00", "ZarIran.Plugin.Payments.DigiPay add refund record")]
	public class SchemaMigration : AutoReversingMigration
	{
		public override void Up()
		{
			Create.TableFor<DigiPayRefundRecord>();

		}
	}
}
