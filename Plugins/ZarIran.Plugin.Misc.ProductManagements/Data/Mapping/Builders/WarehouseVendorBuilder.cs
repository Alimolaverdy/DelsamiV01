using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
{
    public class WarehouseVendorBuilder : NopEntityBuilder<WarehouseVendor>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WarehouseVendor.WarehouseId)).AsInt32().NotNullable()
                .WithColumn(nameof(WarehouseVendor.VendorId)).AsInt32().NotNullable();
        }
    }
}
