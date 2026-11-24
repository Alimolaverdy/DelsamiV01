using FluentMigrator;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Migrations;
using Nop.Data.Extensions;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Data
{
    [NopMigration("2025/10/05 14:35:20", "ZarIran.Plugin.Misc.ExtendedOrder base schema")]
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
