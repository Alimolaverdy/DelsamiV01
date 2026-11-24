using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ZarIran.Plugin.Misc.Core.Domain;

namespace ZarIran.Plugin.Misc.Core.Data.Mapping.Builders
{
    internal class CityBuilder : NopEntityBuilder<City>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(City.StateProvinceId)).AsString(3000).NotNullable()
                .WithColumn(nameof(City.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(City.Featured)).AsBoolean().NotNullable();
        }
    }
}