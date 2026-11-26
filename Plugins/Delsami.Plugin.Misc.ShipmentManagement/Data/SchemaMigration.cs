using FluentMigrator;
using Delsami.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Misc.ShipmentManagement.Data
{
    [NopMigration("2026/11/26 14:38:00", "Delsami.Plugin.Misc.ShipmentManagement base schema")]
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
