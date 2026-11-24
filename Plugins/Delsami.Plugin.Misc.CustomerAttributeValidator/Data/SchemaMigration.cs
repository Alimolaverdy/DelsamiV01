using FluentMigrator;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Domain;
using Nop.Data.Migrations;
using Nop.Data.Extensions;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Data
{
    [NopMigration("2026/11/15 15:40:00", "ZarIran.Plugin.Misc.CustomerAttributeValidator base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<CustomerAttributeValidation>();
        }
    }
}
