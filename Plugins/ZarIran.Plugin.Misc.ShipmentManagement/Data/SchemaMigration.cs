using FluentMigrator;
using ZarIran.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Data
{
    [NopMigration("2026/02/10 14:38:00", "ZarIran.Plugin.Misc.ShipmentManagement base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<ShipmentDelivery>();
            Create.TableFor<ShipmentTrackingNumberImporterReport>();
            Create.TableFor<ShipmentTrackingNumberImport>();
        }
    }
}
