using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
{
    internal class PaymentMethodRewardPointBuilder : NopEntityBuilder<PaymentMethodRewardPoint>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodRewardPoint.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentMethodRewardPoint.RewardPointHistoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentMethodRewardPoint.Status)).AsInt16().NotNullable();
        }
    }
}
