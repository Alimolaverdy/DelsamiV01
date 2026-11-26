using FluentMigrator;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Delsami.Plugin.Misc.ProductManagement.Data
{
    [NopMigration("2026/11/26 14:35:20", "Delsami.Plugin.Misc.ExtendedVendor base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<WarehouseAddressCustomerDistance>();
            Create.TableFor<WarehouseVendor>();
            Create.TableFor<GeneratedVirtualProduct>();
            Create.TableFor<ReservedStockQuantity>();
            Create.TableFor<ReservedStockQuantityHistory>();
            Create.TableFor<BundleProduct>();

        }
    }
}
