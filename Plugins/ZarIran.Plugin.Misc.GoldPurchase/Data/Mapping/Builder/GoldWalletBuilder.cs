using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Data.Mapping.Builder
{
    internal class GoldWalletBuilder : NopEntityBuilder<GoldWallet>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GoldWallet.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(GoldWallet.GoldBalanceGrams)).AsDecimal(18, 8).NotNullable()
                .WithColumn(nameof(GoldWallet.CreatedOnUtc)).AsDateTime().NotNullable()
                .WithColumn(nameof(GoldWallet.UpdatedOnUtc)).AsDateTime().NotNullable();
        }
    }
}
