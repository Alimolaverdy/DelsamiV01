using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Data.Mapping.Builder
{
    public class CustomerAttributeValidationBuilder : NopEntityBuilder<CustomerAttributeValidation>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerAttributeValidation.CustomerAttributeId)).AsInt32().NotNullable()
                .WithColumn(nameof(CustomerAttributeValidation.RegexPattern)).AsString(200).Nullable()
                .WithColumn(nameof(CustomerAttributeValidation.IsNationalCode)).AsBoolean().NotNullable()
                .WithColumn(nameof(CustomerAttributeValidation.Active)).AsBoolean().NotNullable()
                .WithColumn(nameof(CustomerAttributeValidation.PreventFromShoppingInInvalidState)).AsBoolean().NotNullable();
        }
    }
}
