using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
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
