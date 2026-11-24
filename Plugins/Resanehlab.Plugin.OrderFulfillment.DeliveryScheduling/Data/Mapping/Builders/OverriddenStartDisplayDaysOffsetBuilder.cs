using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Data.Mapping.Builders
{
    public class OverriddenStartDisplayDaysOffsetBuilder : NopEntityBuilder<OverriddenStartDisplayDaysOffset>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OverriddenStartDisplayDaysOffset.CategoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(OverriddenStartDisplayDaysOffset.StartDisplayDaysOffset)).AsInt32().NotNullable();
        }
    }
}
