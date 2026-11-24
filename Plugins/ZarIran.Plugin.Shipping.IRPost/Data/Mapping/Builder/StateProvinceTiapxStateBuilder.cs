using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Shipping.IRPost.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Shipping.IRPost.Data.Mapping.Builder
{
    internal class StateProvincePostStateBuilder : NopEntityBuilder<ZarIranPostCityMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ZarIranPostCityMapping.StateProvinceId)).AsInt32().NotNullable()
                .WithColumn(nameof(ZarIranPostCityMapping.Name)).AsString().NotNullable()
                .WithColumn(nameof(ZarIranPostCityMapping.ZarIranCityId)).AsInt32().NotNullable()
                .WithColumn(nameof(ZarIranPostCityMapping.PostCityId)).AsInt32().NotNullable();
        }
    }
}
