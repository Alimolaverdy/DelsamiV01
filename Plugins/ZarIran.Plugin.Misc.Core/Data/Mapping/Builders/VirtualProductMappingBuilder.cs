using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using ZarIran.Plugin.Misc.Core.Domain;

namespace ZarIran.Plugin.Misc.Core.Data.Mapping.Builders
{
    public class VirtualProductMappingBuilder : NopEntityBuilder<VirtualProductMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(VirtualProductMapping.VirtualProductId)).AsInt32().NotNullable()
                .WithColumn(nameof(VirtualProductMapping.MainProductId)).AsInt32().Nullable()
                .WithColumn(nameof(VirtualProductMapping.Deleted)).AsBoolean().Nullable();
        }
    }
}
