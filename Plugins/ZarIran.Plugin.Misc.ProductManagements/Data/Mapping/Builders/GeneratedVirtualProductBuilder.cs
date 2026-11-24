using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
{
    internal class GeneratedVirtualProductBuilder : NopEntityBuilder<GeneratedVirtualProduct>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GeneratedVirtualProduct.Title)).AsString().NotNullable()
                .WithColumn(nameof(GeneratedVirtualProduct.MainParentProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(GeneratedVirtualProduct.GeneratedParentProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(GeneratedVirtualProduct.MainProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(GeneratedVirtualProduct.GeneratedMainProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(GeneratedVirtualProduct.GeneratedVirtualProductId)).AsInt32().Nullable()
                .WithColumn(nameof(GeneratedVirtualProduct.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(GeneratedVirtualProduct.Published)).AsDateTime2().NotNullable();
        }
    }
}
