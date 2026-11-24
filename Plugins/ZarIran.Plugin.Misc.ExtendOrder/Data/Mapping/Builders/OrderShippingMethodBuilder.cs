using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.ExtendedOrder.Domian;

namespace ZarIran.Plugin.Misc.ExtendedOrder.Data.Mapping.Builders
{
    internal class OrderShippingMethodBuilder : NopEntityBuilder<OrderShippingMethod>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderShippingMethod.OrderId)).AsInt32().NotNullable()
                .WithColumn(nameof(OrderShippingMethod.ShippingMethodId)).AsInt32().NotNullable();
        }
    }
}
