using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
{
    internal class BackInStockSubscriptionBuilder : NopEntityBuilder<ZarIranBackInStockSubscription>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ZarIranBackInStockSubscription.ProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(ZarIranBackInStockSubscription.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(ZarIranBackInStockSubscription.MobileNumber)).AsString().NotNullable()
                .WithColumn(nameof(ZarIranBackInStockSubscription.CreatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(ZarIranBackInStockSubscription.UpdatedOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(ZarIranBackInStockSubscription.IsNotified)).AsBoolean().NotNullable()
                .WithColumn(nameof(ZarIranBackInStockSubscription.IsActive)).AsBoolean().NotNullable();
        }
    }
}
