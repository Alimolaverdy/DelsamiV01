using FluentMigrator;
using Delsami.Plugin.Shipping.Tipax.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Shipping.Tipax.Data
{
    [NopMigration("2026/11/26 11:11:00", "Delsami.Plugin.Shipping.Tipax base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<DelsamiTipaxCityMapping>();
        }
    }
}
