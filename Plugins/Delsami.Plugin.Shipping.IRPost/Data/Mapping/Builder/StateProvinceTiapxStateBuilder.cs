using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Shipping.IRPost.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Shipping.IRPost.Data.Mapping.Builder
{
    internal class StateProvincePostStateBuilder : NopEntityBuilder<DelsamiPostCityMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DelsamiPostCityMapping.StateProvinceId)).AsInt32().NotNullable()
                .WithColumn(nameof(DelsamiPostCityMapping.Name)).AsString().NotNullable()
                .WithColumn(nameof(DelsamiPostCityMapping.DelsamiCityId)).AsInt32().NotNullable()
                .WithColumn(nameof(DelsamiPostCityMapping.PostCityId)).AsInt32().NotNullable();
        }
    }
}
