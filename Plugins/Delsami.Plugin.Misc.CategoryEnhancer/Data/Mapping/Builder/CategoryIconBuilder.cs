using FluentMigrator.Builders.Create.Table;
using Delsami.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data.Mapping.Builders;

namespace Delsami.Plugin.Misc.CategoryEnhancer.Data.Mapping.Builder
{
    public class CategoryIconBuilder : NopEntityBuilder<CategoryIcon>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(CategoryIcon.CategoryId)).AsInt32().NotNullable()
                 .WithColumn(nameof(CategoryIcon.IconId)).AsInt32().NotNullable();
        }
    }
}