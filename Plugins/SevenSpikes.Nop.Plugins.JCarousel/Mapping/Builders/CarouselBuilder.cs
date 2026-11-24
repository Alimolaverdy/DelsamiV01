using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;

namespace SevenSpikes.Nop.Plugins.JCarousel.Mapping.Builders;

public class CarouselBuilder : NopEntityBuilder<Carousel>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Title").AsString(250).Nullable()
            .WithColumn("CarouselType").AsInt32().NotNullable();
    }
}
