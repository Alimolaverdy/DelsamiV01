using System.Collections.Generic;
using System.Threading.Tasks;
using SevenSpikes.Nop.Plugins.JCarousel.Domain;

namespace SevenSpikes.Nop.Plugins.JCarousel.Services;

public interface IJCarouselService
{
    Task InsertCarouselAsync(Carousel carousel);

    Task UpdateCarouselAsync(Carousel carousel);

    Task DeleteCarouselAsync(Carousel carousel);

    Task<IList<Carousel>> GetAllCarouselsAsync();

    Task<Carousel> GetCarouselByNameAsync(string carouselName);

    Task<Carousel> GetCarouselByIdAsync(int carouselId);

    Task<IList<Carousel>> GetCarouselsByWidgetZoneAsync(string widgetZone);
}
