using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Data.Mapping.Builder;

public class GoldWithdrawalBuilder : NopEntityBuilder<GoldWithdrawal>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(GoldWithdrawal.RequestKey)).AsGuid().NotNullable().Unique()
            .WithColumn(nameof(GoldWithdrawal.CustomerId)).AsInt32().NotNullable()
            .WithColumn(nameof(GoldWithdrawal.StoreId)).AsInt32().NotNullable()
            .WithColumn(nameof(GoldWithdrawal.CustomerName)).AsString(400).NotNullable()
            .WithColumn(nameof(GoldWithdrawal.Phone)).AsString(400).NotNullable()
            .WithColumn(nameof(GoldWithdrawal.Sheba)).AsString(26).NotNullable()
            .WithColumn(nameof(GoldWithdrawal.Amount)).AsInt64().NotNullable()
            .WithColumn(nameof(GoldWithdrawal.CreatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(GoldWithdrawal.CompletedOnUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(GoldWithdrawal.CompletedByCustomerId)).AsInt32().Nullable();
    }
}
