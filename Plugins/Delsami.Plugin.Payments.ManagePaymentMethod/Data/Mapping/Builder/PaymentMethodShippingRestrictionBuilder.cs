using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
{
    public class PaymentMethodShippingRestrictionBuilder : NopEntityBuilder<PaymentMethodShippingRestriction>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodShippingRestriction.PaymentMethodSystemName)).AsString().NotNullable()
                .WithColumn(nameof(PaymentMethodShippingRestriction.ShippingMethodId)).AsInt32().NotNullable();
        }
    }
}
