using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using PaymentDomain = ZarIran.Plugin.Payment.Core.Domain;

namespace ZarIran.Plugin.Payment.Core.Data
{
    [NopMigration("2023/10/13 14:38:00", "ZarIran.Plugin.Payment.Core base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<PaymentDomain.Payment>();
        }
    }
}
