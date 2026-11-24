using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.ProductManagements.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.ProductManagements.Data.Mapping.Builders
{
    public class WarehouseAddressCustomerDistanceBuilder : NopEntityBuilder<WarehouseAddressCustomerDistance>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WarehouseAddressCustomerDistance.WarehouseAddressId)).AsInt32().NotNullable()
                .WithColumn(nameof(WarehouseAddressCustomerDistance.CustomerAddressId)).AsInt32().NotNullable()
                .WithColumn(nameof(WarehouseAddressCustomerDistance.Distance)).AsInt32().NotNullable();
        }
    }
}
