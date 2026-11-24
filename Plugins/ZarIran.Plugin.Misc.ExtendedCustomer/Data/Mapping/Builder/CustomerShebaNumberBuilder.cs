using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Data.Mapping.Builder
{
    public class CustomerShebaNumberBuilder : NopEntityBuilder<CustomerShebaNumber>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerShebaNumber.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(CustomerShebaNumber.Number)).AsString(200).NotNullable();
        }
    }
}
