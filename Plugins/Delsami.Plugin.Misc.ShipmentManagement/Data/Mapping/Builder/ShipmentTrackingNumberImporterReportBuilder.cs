using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.ShipmentManagement.Data.Mapping.Builder
{
    public class ShipmentTrackingNumberImporterReportBuilder : NopEntityBuilder<ShipmentTrackingNumberImporterReport>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.ShipmentId)).AsInt32().Nullable()
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.OrderId)).AsInt32().Nullable()
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.Status)).AsInt32().NotNullable()
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.ShipmentTrackingNumberImportId)).AsInt32().NotNullable()
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.TrackingNumber)).AsString().NotNullable()
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.ErrorDescription)).AsString().Nullable()
                .WithColumn(nameof(ShipmentTrackingNumberImporterReport.CustomerId)).AsInt32().NotNullable();
        }
    }
}
