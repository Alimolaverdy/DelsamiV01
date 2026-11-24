using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
{
    internal class BundleProductBuilder : NopEntityBuilder<BundleProduct>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BundleProduct.ProductId1)).AsInt32().NotNullable()
                .WithColumn(nameof(BundleProduct.ProductId2)).AsInt32().Nullable()
                .WithColumn(nameof(BundleProduct.Count)).AsInt32().NotNullable()
                .WithColumn(nameof(BundleProduct.Price)).AsDecimal().NotNullable();
        }
    }
}