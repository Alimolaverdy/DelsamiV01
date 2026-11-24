using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
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
