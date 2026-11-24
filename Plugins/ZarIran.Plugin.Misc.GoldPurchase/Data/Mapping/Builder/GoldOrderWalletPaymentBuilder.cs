using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ZarIran.Plugin.Misc.GoldPurchase.Domain;

namespace ZarIran.Plugin.Misc.GoldPurchase.Data.Mapping.Builder;

public class GoldOrderWalletPaymentBuilder : NopEntityBuilder<GoldOrderWalletPayment>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(GoldOrderWalletPayment.OrderId)).AsInt32().NotNullable().Unique()
            .WithColumn(nameof(GoldOrderWalletPayment.Points)).AsInt32().NotNullable()
            .WithColumn(nameof(GoldOrderWalletPayment.SalePoints)).AsInt32().NotNullable()
            .WithColumn(nameof(GoldOrderWalletPayment.Refunded)).AsBoolean().NotNullable();
    }
}
