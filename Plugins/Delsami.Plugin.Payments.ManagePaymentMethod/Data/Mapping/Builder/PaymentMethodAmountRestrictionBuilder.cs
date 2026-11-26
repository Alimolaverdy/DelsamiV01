using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
{
    public class PaymentMethodAmountRestrictionBuilder : NopEntityBuilder<PaymentMethodAmountRestriction>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodAmountRestriction.PaymentMethodSystemName)).AsString().NotNullable()
                .WithColumn(nameof(PaymentMethodAmountRestriction.MinAmount)).AsDecimal().Nullable()
                .WithColumn(nameof(PaymentMethodAmountRestriction.MaxAmount)).AsDecimal().Nullable();
        }
    }
}
