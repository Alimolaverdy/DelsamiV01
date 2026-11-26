using FluentMigrator;
using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Migrations;
using Nop.Data.Extensions;

namespace Delsami.Plugin.Misc.ExtendedOrder.Data
{
    [NopMigration("2026/11/26 14:35:20", "Delsami.Plugin.Misc.ExtendedOrder base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<OrderInvoice>();
            Create.TableFor<OrderSubStatus>();
            Create.TableFor<OrderSubStatusMapping>();
            Create.TableFor<OrderShippingMethod>();
            Create.TableFor<OrderWarehouseLocation>();
        }
    }
}
