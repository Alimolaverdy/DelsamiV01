using FluentMigrator;
using ZarIran.Plugin.Shipping.Tipax.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Shipping.Tipax.Data
{
    [NopMigration("2025/09/06 11:11:00", "ZarIran.Plugin.Shipping.Tipax base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<ZarIranTipaxCityMapping>();
        }
    }
}
