using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
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
