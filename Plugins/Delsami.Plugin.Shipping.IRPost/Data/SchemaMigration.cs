using FluentMigrator;
using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Shipping.IRPost.Data
{
    [NopMigration("2026/11/26 09:38:00", "add postCityMaping")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<PriceRange>();
            Create.TableFor<DelsamiPostCityMapping>();
        }
    }
}
