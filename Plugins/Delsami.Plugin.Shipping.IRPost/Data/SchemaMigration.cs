using FluentMigrator;
using ZarIran.Plugin.Shipping.IRPost.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Shipping.IRPost.Data
{
    [NopMigration("2026/06/14 09:38:00", "add postCityMaping")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<PriceRange>();
            Create.TableFor<ZarIranPostCityMapping>();
        }
    }
}
