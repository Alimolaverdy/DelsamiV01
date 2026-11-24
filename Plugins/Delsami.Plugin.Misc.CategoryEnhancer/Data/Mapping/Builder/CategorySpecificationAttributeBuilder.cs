using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Data.Mapping.Builder
{
    public class CategorySpecificationAttributeBuilder : NopEntityBuilder<CategorySpecificationAttribute>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CategorySpecificationAttribute.CategoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(CategorySpecificationAttribute.SpecificationAttributeId)).AsInt32().NotNullable();
        }
    }
}
