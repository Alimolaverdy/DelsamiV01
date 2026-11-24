using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using SevenSpikes.Nop.Mappings.Domain;
using SevenSpikes.Nop.Mappings.Domain.Enums;
using SevenSpikes.Nop.Plugins.JCarousel.Infrastructure.Constants;

namespace SevenSpikes.Nop.Plugins.JCarousel.Services;

public class ItemsMappingService : IItemsMappingService
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IManufacturerService _manufacturerService;
    private readonly IVendorService _vendorService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IAclService _aclService;
    private readonly IRepository<EntityMapping> _entityMappingRepository;

    public ItemsMappingService(
        IProductService productService,
        ICategoryService categoryService,
        IManufacturerService manufacturerService,
        IVendorService vendorService,
        IStoreMappingService storeMappingService,
        IAclService aclService,
        IRepository<EntityMapping> entityMappingRepository)
    {
        _productService = productService;
        _categoryService = categoryService;
        _manufacturerService = manufacturerService;
        _vendorService = vendorService;
        _storeMappingService = storeMappingService;
        _aclService = aclService;
        _entityMappingRepository = entityMappingRepository;
    }

    public async Task<IList<Product>> GetProductsMappedToTheJCarouselAsync(int jCarouselId)
    {
        var productIds = GetListOfIdsForType(jCarouselId, MappingType.Product).ToArray();
        var products = await _productService.GetProductsByIdsAsync(productIds);
        var result = new List<Product>(products.Count);

        foreach (var product in products)
        {
            if (product.Deleted || !product.Published)
                continue;

            if (!await _storeMappingService.AuthorizeAsync<Product>(product))
                continue;

            if (!await _aclService.AuthorizeAsync<Product>(product))
                continue;

            result.Add(product);
        }

        return result;
    }

    public async Task<List<Category>> GetCategoriesMappedToTheJCarouselAsync(int jCarouselId)
    {
        var result = new List<Category>();
        var categoryIds = GetListOfIdsForType(jCarouselId, MappingType.Category);

        foreach (var categoryId in categoryIds)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (category == null || category.Deleted || !category.Published)
                continue;

            if (!await _storeMappingService.AuthorizeAsync<Category>(category))
                continue;

            if (!await _aclService.AuthorizeAsync<Category>(category))
                continue;

            result.Add(category);
        }

        return result;
    }

    public async Task<List<Manufacturer>> GetManufacturersMappedToTheJCarouselAsync(int jCarouselId)
    {
        var result = new List<Manufacturer>();
        var manufacturerIds = GetListOfIdsForType(jCarouselId, MappingType.Manufacturer);

        foreach (var manufacturerId in manufacturerIds)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted || !manufacturer.Published)
                continue;

            if (!await _storeMappingService.AuthorizeAsync<Manufacturer>(manufacturer))
                continue;

            if (!await _aclService.AuthorizeAsync<Manufacturer>(manufacturer))
                continue;

            result.Add(manufacturer);
        }

        return result;
    }

    public async Task<List<Vendor>> GetVendorsMappedToTheJCarouselAsync(int jCarouselId)
    {
        var result = new List<Vendor>();
        var vendorIds = GetListOfIdsForType(jCarouselId, MappingType.Vendor);

        foreach (var vendorId in vendorIds)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
            if (vendor != null && !vendor.Deleted && vendor.Active)
                result.Add(vendor);
        }

        return result;
    }

    private IEnumerable<int> GetListOfIdsForType(int jCarouselId, MappingType type)
    {
        return from mapping in _entityMappingRepository.Table
               where mapping.EntityId == jCarouselId
                     && (int)mapping.EntityType == (int)Plugin.EntityType
                     && (int)mapping.MappingType == (int)type
               orderby mapping.DisplayOrder
               select mapping.MappedEntityId;
    }
}
