using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using SevenSpikes.Nop.Plugins.JCarousel.Domain.Enums;

namespace SevenSpikes.Nop.Plugins.JCarousel.Domain;

public class Carousel : BaseEntity, ILocalizedEntity, IStoreMappingSupported
{
    public string Name { get; set; }

    public string Title { get; set; }

    public string DataSourceType { get; set; }

    public int DataSourceEntityId { get; set; }

    public CarouselType CarouselType { get; set; }

    public JCarouselEntitySettings JCarouselEntitySettings { get; set; }

    public bool LimitedToStores { get; set; }
}
