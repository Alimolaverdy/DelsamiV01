using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Shipping.Tipax.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Shipping.Tipax.Data.Mapping.Builder
{
    internal class StateProvinceTiapxStateBuilder : NopEntityBuilder<ZarIranTipaxCityMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ZarIranTipaxCityMapping.StateProvinceId)).AsInt32().NotNullable()
                .WithColumn(nameof(ZarIranTipaxCityMapping.Name)).AsString().NotNullable()
                .WithColumn(nameof(ZarIranTipaxCityMapping.ZarIranCityId)).AsInt32().NotNullable()
                .WithColumn(nameof(ZarIranTipaxCityMapping.TipaxCityId)).AsInt32().NotNullable();
        }
    }
}
