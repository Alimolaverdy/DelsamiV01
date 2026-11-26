using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.ProductManagement.Data.Mapping.Builders
{
    public class ReservedStockQuantityBuilder : NopEntityBuilder<ReservedStockQuantity>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReservedStockQuantity.ProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(ReservedStockQuantity.LastModifiedDateTime)).AsDateTime2().NotNullable()
                .WithColumn(nameof(ReservedStockQuantity.Count)).AsInt32().NotNullable();
        }
    }
}
