using ZarIran.Plugin.Misc.Core.Enums;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ProductManagements.Services
{
    public interface ICustomZarIranProductService
	{
		Task<IPagedList<Product>> SearchProductsAsync(
		   int pageIndex = 0,
		   int pageSize = int.MaxValue,
		   IList<int> categoryIds = null,
		   IList<int> manufacturerIds = null,
		   int storeId = 0,
		   int vendorId = 0,
		   int warehouseId = 0,
		   ProductTypes? productType = null,
		   bool visibleIndividuallyOnly = false,
		   bool excludeFeaturedProducts = false,
		   decimal? priceMin = null,
		   decimal? priceMax = null,
		   int productTagId = 0,
		   string keywords = null,
		   bool searchDescriptions = false,
		   bool searchManufacturerPartNumber = true,
		   bool searchSku = true,
		   bool searchProductTags = false,
		   int languageId = 0,
		   IList<SpecificationAttributeOption> filteredSpecOptions = null,
		   ProductSortingEnum orderBy = ProductSortingEnum.Position,
		   bool showHidden = false,
		   bool? overridePublished = null);


		Task ImportAsync(List<ProductSpecificationAttribute> records);

		Task<List<Product>> GetProductsByName(List<string> ProductNames);

		Task<List<Product>> GetAllProductsByIds(List<int> ids);

	}


}
