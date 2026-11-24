using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ShipmentManagement.Domain;
using Nop.Core;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ShipmentManagement.Data.Mapping.Builder
{
    public class ShipmentDeliveryBuilder : NopEntityBuilder<ShipmentDelivery>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShipmentDelivery.ShipmentId)).AsInt32().NotNullable()
                .WithColumn(nameof(ShipmentDelivery.DeliveryCode)).AsInt32().NotNullable()
                .WithColumn(nameof(ShipmentDelivery.Delivered)).AsBoolean().NotNullable()
                .WithColumn(nameof(ShipmentDelivery.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(ShipmentDelivery.Shipped)).AsBoolean().NotNullable()
                .WithColumn(nameof(ShipmentDelivery.ShippedOnUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(ShipmentDelivery.DeliveredOnUtc)).AsDateTime2().Nullable();
        }
    }
}
