using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Data.Mapping.Builder
{
    public class PaymentMethodCustomerRoleRestrictionBuilder : NopEntityBuilder<PaymentMethodCustomerRoleRestriction>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PaymentMethodCustomerRoleRestriction.PaymentMethodSystemName)).AsString().NotNullable()
                .WithColumn(nameof(PaymentMethodCustomerRoleRestriction.CustomerRoleId)).AsInt32().NotNullable();
        }
    } 
}
