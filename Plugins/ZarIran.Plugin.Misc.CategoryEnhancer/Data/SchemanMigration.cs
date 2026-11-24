using FluentMigrator;
using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Data
{
    [NopMigration("2026/11/07 15:38:00", "ZarIran.Plugin.Misc.CategoryEnhancer base schema")]
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
