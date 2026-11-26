using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Shipping.Tipax.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Shipping.Tipax.Data.Mapping.Builder
{
    internal class StateProvinceTiapxStateBuilder : NopEntityBuilder<DelsamiTipaxCityMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DelsamiTipaxCityMapping.StateProvinceId)).AsInt32().NotNullable()
                .WithColumn(nameof(DelsamiTipaxCityMapping.Name)).AsString().NotNullable()
                .WithColumn(nameof(DelsamiTipaxCityMapping.DelsamiCityId)).AsInt32().NotNullable()
                .WithColumn(nameof(DelsamiTipaxCityMapping.TipaxCityId)).AsInt32().NotNullable();
        }
    }
}
