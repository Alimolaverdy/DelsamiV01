using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
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
