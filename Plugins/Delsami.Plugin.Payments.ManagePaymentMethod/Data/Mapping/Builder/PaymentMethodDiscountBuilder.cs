using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
{
    public class PaymentMethodDiscountBuilder : NopEntityBuilder<PaymentMethodDiscount>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodDiscount.PaymentMethodSystemName)).AsString().NotNullable()
                .WithColumn(nameof(PaymentMethodDiscount.DiscountPercent)).AsInt32().NotNullable()
                .WithColumn(nameof(PaymentMethodDiscount.StartDateTimeOnUtc)).AsDateTime2().NotNullable()
                .WithColumn(nameof(PaymentMethodDiscount.EndDateTimeOnUtc)).AsDateTime2().Nullable();
        }
    }
}
