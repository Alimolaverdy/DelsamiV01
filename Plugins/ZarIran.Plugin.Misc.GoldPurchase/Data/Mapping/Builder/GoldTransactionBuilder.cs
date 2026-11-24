using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Data.Mapping.Builder
{
    internal class GoldTransactionBuilder : NopEntityBuilder<GoldTransaction>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GoldTransaction.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(GoldTransaction.OrderId)).AsInt32().Nullable()
                .WithColumn(nameof(GoldTransaction.GoldWeightGrams)).AsDecimal(18, 8).NotNullable()
                .WithColumn(nameof(GoldTransaction.PricePerGram)).AsDecimal(18, 4).NotNullable()
                .WithColumn(nameof(GoldTransaction.Amount)).AsDecimal(18, 4).NotNullable()
                .WithColumn(nameof(GoldTransaction.TransactionType)).AsInt16().NotNullable()
                .WithColumn(nameof(GoldTransaction.GoldBalanceAfterGrams)).AsDecimal(18, 8).NotNullable().WithDefaultValue(0)
                .WithColumn(nameof(GoldTransaction.Note)).AsString(1000).Nullable()
                .WithColumn(nameof(GoldTransaction.AdminCustomerId)).AsInt32().Nullable()
                .WithColumn(nameof(GoldTransaction.CreatedOnUtc)).AsDateTime2().NotNullable();
        }
    }
}
