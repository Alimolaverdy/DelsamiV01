using FluentMigrator;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace ZarIran.Plugin.Misc.ProductManagements.Data
{
    [NopMigration("2026/11/10 14:35:20", "ZarIran.Plugin.Misc.ProductManagements base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<WarehouseAddressCustomerDistance>();
            Create.TableFor<WarehouseVendor>();
            Create.TableFor<GeneratedVirtualProduct>();
            Create.TableFor<ReservedStockQuantity>();
            Create.TableFor<ReservedStockQuantityHistory>();
            Create.TableFor<ZarIranBackInStockSubscription>();
            Create.TableFor<OtherBrandProduct>();
            Create.TableFor<BundleProduct>();
            Create.TableFor<ProductPricing>();

        }
    }
}
