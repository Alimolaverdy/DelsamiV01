using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.ExtendedOrder.Data.Mapping.Builders
{
    public class OrderSubStatusMappingBuilder : NopEntityBuilder<OrderSubStatusMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(OrderSubStatusMapping.OrderId)).AsInt32().NotNullable()
                 .WithColumn(nameof(OrderSubStatusMapping.SubStatusId)).AsInt32().NotNullable()
                 .WithColumn(nameof(OrderSubStatusMapping.CreatedDateTime)).AsDateTime().NotNullable();
        }
    }
}
