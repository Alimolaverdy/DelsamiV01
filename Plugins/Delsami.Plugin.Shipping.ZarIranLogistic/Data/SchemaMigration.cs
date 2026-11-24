using FluentMigrator;
using ZarIran.Plugin.Shipping.ZarIranLogistic.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Data
{
    [NopMigration("2025/11/02 12:00:00", "ZarIran.Plugin.Shipping.ZarIranLogistic base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<QueuedCanceledShipmentForSendToLogistic>();
        }
    }
}
