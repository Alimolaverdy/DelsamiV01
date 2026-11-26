using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Shipping.IRPost.Data.Mapping.Builder
{
    public class PriceRangeBuilder : NopEntityBuilder<PriceRange>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PriceRange.MinWieght)).AsDecimal().NotNullable()
                .WithColumn(nameof(PriceRange.MaxWeight)).AsDecimal().Nullable()
                .WithColumn(nameof(PriceRange.NeighbouringPrice)).AsDecimal().NotNullable()
                .WithColumn(nameof(PriceRange.NonNeighbouringPrice)).AsDecimal().NotNullable()
                .WithColumn(nameof(PriceRange.ToUpperWeight)).AsBoolean().NotNullable()
                .WithColumn(nameof(PriceRange.ModifierCustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(PriceRange.StartDateUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(PriceRange.EndDateUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(PriceRange.Deleted)).AsBoolean().NotNullable();

        }
    }
}
