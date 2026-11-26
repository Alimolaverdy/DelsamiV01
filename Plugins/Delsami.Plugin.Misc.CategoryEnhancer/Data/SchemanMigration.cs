using FluentMigrator;
using Delsami.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Data
{
    [NopMigration("2026/11/07 15:38:00", "Delsami.Plugin.Misc.CategoryEnhancer base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<CategoryIcon>();
            Create.TableFor<CanonicalCategory>();
            Create.TableFor<CategorySpecificationAttribute>();

        }
    }
}
