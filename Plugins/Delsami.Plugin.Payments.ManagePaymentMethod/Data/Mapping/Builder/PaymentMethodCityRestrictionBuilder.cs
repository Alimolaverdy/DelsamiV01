using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
{
    public class PaymentMethodCityRestrictionBuilder : NopEntityBuilder<PaymentMethodCityRestriction>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodCityRestriction.PaymentMethodSystemName)).AsString().NotNullable()
                .WithColumn(nameof(PaymentMethodCityRestriction.CityId)).AsInt32().NotNullable();
        }
    } 
}
