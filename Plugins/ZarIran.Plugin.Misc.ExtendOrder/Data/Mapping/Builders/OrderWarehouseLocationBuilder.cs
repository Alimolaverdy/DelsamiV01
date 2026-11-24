using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Data.Mapping.Builders
{
    public class OrderWarehouseLocationBuilder : NopEntityBuilder<OrderWarehouseLocation>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderWarehouseLocation.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(OrderWarehouseLocation.Location)).AsInt32().NotNullable();
        }
    }
}
