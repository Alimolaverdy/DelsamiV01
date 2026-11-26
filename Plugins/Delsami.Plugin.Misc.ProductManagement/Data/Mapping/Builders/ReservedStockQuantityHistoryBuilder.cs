using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.ProductManagement.Data.Mapping.Builders
{
    public class ReservedStockQuantityHistoryBuilder : NopEntityBuilder<ReservedStockQuantityHistory>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReservedStockQuantityHistory.ProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(ReservedStockQuantityHistory.OrderId)).AsString().NotNullable()
                .WithColumn(nameof(ReservedStockQuantityHistory.ProductStockQuantity)).AsInt32().NotNullable()
                .WithColumn(nameof(ReservedStockQuantityHistory.Count)).AsInt32().NotNullable()
                .WithColumn(nameof(ReservedStockQuantityHistory.CountAdjustment)).AsInt32().NotNullable()
                .WithColumn(nameof(ReservedStockQuantityHistory.Message)).AsString().NotNullable()
                .WithColumn(nameof(ReservedStockQuantityHistory.CreatedDateTime)).AsDateTime2().NotNullable();
        }
    }
}
