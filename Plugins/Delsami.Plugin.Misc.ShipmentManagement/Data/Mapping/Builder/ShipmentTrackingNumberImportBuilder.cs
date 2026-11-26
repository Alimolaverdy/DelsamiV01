using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.ShipmentManagement.Data.Mapping.Builder
{
    public class ShipmentTrackingNumberImportBuilder : NopEntityBuilder<ShipmentTrackingNumberImport>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShipmentTrackingNumberImport.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(ShipmentTrackingNumberImport.BinaryData)).AsBinary().NotNullable();
        }
    }
}
