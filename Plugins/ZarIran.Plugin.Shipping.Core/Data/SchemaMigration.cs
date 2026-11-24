using FluentMigrator;
using ZarIran.Plugin.Shipping.Core.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Shipping.Core.Data
{
    [NopMigration("2026/06/27 18:38:00", "ZarIran.Plugin.Misc.ShippingCore base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<Carton>();
            Create.TableFor<LogisticsScheduleException>();
            Create.TableFor<LogisticsSchedule>();
        }
    }
}
