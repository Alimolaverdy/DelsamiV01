using FluentMigrator;
using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Data
{
    [NopMigration("2026/11/22 14:51:00", "ZarIran.Plugin.Misc.ExtendedCustomer base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<CustomerShebaNumber>();
        }
    }
}
