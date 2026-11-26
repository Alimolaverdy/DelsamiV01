using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ExtendedOrder.Data.Mapping.Builders
{
    public class OrderSubStatusBuilder : NopEntityBuilder<OrderSubStatus>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(OrderSubStatus.Name)).AsString().NotNullable()
                 .WithColumn(nameof(OrderSubStatus.SystemName)).AsString().NotNullable();
        }
    }
}
