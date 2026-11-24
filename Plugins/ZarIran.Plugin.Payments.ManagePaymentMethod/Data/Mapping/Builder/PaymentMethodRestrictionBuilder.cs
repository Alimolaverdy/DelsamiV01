using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
{
    public class PaymentMethodRestrictionBuilder : NopEntityBuilder<PaymentMethodCityRestriction>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodCityRestriction.PaymentMethodSystemName)).AsString().NotNullable()
                .WithColumn(nameof(PaymentMethodCityRestriction.CityId)).AsInt32().NotNullable();
        }
    }
}
