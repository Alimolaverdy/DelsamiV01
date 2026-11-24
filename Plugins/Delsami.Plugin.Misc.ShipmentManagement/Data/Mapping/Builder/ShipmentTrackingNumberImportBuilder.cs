using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Data.Mapping.Builder
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
