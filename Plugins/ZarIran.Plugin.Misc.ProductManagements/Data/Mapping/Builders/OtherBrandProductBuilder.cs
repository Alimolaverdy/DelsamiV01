using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
{
    internal class OtherBrandProductBuilder : NopEntityBuilder<OtherBrandProduct>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OtherBrandProduct.ProductId1)).AsInt32().NotNullable()
                .WithColumn(nameof(OtherBrandProduct.ProductId2)).AsInt32().Nullable()
                .WithColumn(nameof(OtherBrandProduct.DisplayOrder)).AsInt32().NotNullable();
        }
    }
}