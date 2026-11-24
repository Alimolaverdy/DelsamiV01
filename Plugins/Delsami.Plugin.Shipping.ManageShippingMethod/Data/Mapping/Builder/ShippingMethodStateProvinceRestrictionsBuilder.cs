using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Shipping.ManageShippingMethod.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Shipping.ManageShippingMethod.Data.Mapping.Builder
{
    internal class ShippingMethodStateProvinceRestrictionsBuilder : NopEntityBuilder<ShippingMethodCityRestriction>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShippingMethodCityRestriction.ShippingMethodId)).AsInt32().NotNullable()
                .WithColumn(nameof(ShippingMethodCityRestriction.CityId)).AsInt32().NotNullable();
        }
    }
}