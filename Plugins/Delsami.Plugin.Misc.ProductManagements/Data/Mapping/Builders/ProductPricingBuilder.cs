using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
{
    internal class ProductPricingBuilder : NopEntityBuilder<ProductPricing>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(ProductPricing.ProductId)).AsInt64().NotNullable()
                 .WithColumn(nameof(ProductPricing.OldMakingFee)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.MakingFee)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.OldProfitPercent)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.ProfitPercent)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.OldCoWorkerMakingFee)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.CoWorkerMakingFee)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.OldCoWorkerProfitPercent)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.AdditionalAmount)).AsDecimal().NotNullable()
                 .WithColumn(nameof(ProductPricing.CoWorkerProfitPercent)).AsDecimal().NotNullable();

        }
    }
}