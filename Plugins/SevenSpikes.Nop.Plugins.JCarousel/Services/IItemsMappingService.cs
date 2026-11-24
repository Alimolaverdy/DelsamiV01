using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;

namespace SevenSpikes.Nop.Plugins.JCarousel.Services;

public interface IItemsMappingService
{
    Task<IList<Product>> GetProductsMappedToTheJCarouselAsync(int jCarouselId);

    Task<List<Category>> GetCategoriesMappedToTheJCarouselAsync(int jCarouselId);

    Task<List<Manufacturer>> GetManufacturersMappedToTheJCarouselAsync(int jCarouselId);

    Task<List<Vendor>> GetVendorsMappedToTheJCarouselAsync(int jCarouselId);
}
