using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.ExtendedOrder.Domian;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.ExtendedOrder.Data.Mapping.Builders
{
    public class OrderInvoiceBuilder : NopEntityBuilder<OrderInvoice>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(OrderInvoice.InvoiceId)).AsInt32().NotNullable()
                .WithColumn(nameof(OrderInvoice.OrderId)).AsInt32().NotNullable();
        }
    }
}
