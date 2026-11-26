using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.ProductManagement.Data.Mapping.Builders
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
