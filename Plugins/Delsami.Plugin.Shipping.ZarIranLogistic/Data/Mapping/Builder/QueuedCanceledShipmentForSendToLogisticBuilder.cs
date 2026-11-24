using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Shipping.ZarIranLogistic.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Shipping.ZarIranLogistic.Data.Mapping.Builder
{
    internal class QueuedCanceledShipmentForSendToLogisticBuilder : NopEntityBuilder<QueuedCanceledShipmentForSendToLogistic>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.ShipmentId)).AsInt32().NotNullable()
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.SentTries)).AsInt32().NotNullable()
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.Sent)).AsBoolean().NotNullable()
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.Result)).AsString().Nullable()
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.ResultStatusCode)).AsInt32().Nullable()
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.CreatedDateTime)).AsDateTime2().NotNullable()
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.lastTryDateTime)).AsDateTime2().Nullable()
                .WithColumn(nameof(QueuedCanceledShipmentForSendToLogistic.SentDateTime)).AsDateTime2().Nullable();
        }
    }
}
