using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Misc.CategoryEnhancer.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Misc.CategoryEnhancer.Data.Mapping.Builder
{
    public class CanonicalCategoryBuilder : NopEntityBuilder<CanonicalCategory>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CanonicalCategory.CategoryId)).AsInt32().NotNullable()
                .WithColumn(nameof(CanonicalCategory.CanonicalCategoryId)).AsInt32().NotNullable();
        }
    }
}
