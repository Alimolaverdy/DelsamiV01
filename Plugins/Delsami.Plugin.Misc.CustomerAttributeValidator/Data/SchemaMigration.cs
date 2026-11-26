using FluentMigrator;
using Delsami.Plugin.Misc.CustomerAttributeValidator.Domain;
using Nop.Data.Migrations;
using Nop.Data.Extensions;

namespace Delsami.Plugin.Misc.CustomerAttributeValidator.Data
{
    [NopMigration("2026/11/26 15:40:00", "Delsami.Plugin.Misc.CustomerAttributeValidator base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<CustomerAttributeValidation>();
        }
    }
}
