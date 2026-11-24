using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Widgets.AddressLocation.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Widgets.AddressLocation.Data.Mapping.Builder
{
    internal class AddressCityBuilder : NopEntityBuilder<AddressCity>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(AddressCity.AddressId)).AsInt16().NotNullable()
                 .WithColumn(nameof(AddressCity.CityId)).AsInt16().NotNullable();
        }
    }
}
